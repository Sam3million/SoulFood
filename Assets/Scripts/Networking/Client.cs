using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking;
using Networking.Packets;
using ProtoBuf;
using ProtoBuf.Meta;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using TcpClient = NetCoreServer.TcpClient;

public class Client : TcpClient
{
    public Client(IPAddress address, int port) : base(address, port)
    {
    }

    public Client(string address, int port) : base(address, port)
    {
    }

    public Client(DnsEndPoint endpoint) : base(endpoint)
    {
    }

    public Client(IPEndPoint endpoint) : base(endpoint)
    {
    }
    
    public int SynchronousReceiveBufferSize = 8192;
    private NativeArray<byte> receiveBuffer;
    private unsafe byte* receiveBufferPtr;

    public NativeArray<byte> messageBuffer;
    public unsafe byte* messageBufferPtr;

    private Dictionary<Guid, NetworkObject> networkObjects = new();
    
    // For client requests that need a response from the server, this stores the callback to call upon receiving the response
    private Dictionary<uint, MulticastDelegate> requestDict = new();
    public uint RequestCount;

    public static Client Instance;
    
    public override bool Connect()
    {
        bool result = base.Connect();
        if (result)
        {
            receiveBuffer = new NativeArray<byte>(SynchronousReceiveBufferSize, Allocator.Persistent);
            messageBuffer = new NativeArray<byte>(16384, Allocator.Persistent);
            unsafe
            {
                receiveBufferPtr = (byte*) receiveBuffer.GetUnsafePtr();
                messageBufferPtr = (byte*) messageBuffer.GetUnsafePtr();
            }
        }
        return result;
    }

    public unsafe long Receive()
    {
        if (Socket != null && Socket.Available > 0)
        {
            if (!IsConnected)
                return 0;
            SocketError errorCode;
            long size1 = Socket.Receive(new Span<byte>(receiveBufferPtr, receiveBuffer.Length), SocketFlags.None, out errorCode);
            if (size1 > 0L)
            {
                OnReceived(size1);
            }
            if (errorCode != SocketError.Success)
            {
                Debug.LogError("Socket error: " + errorCode);
                Disconnect();
            }
            return size1;
        }
        return 0;
    }

    public int currentMessageSize = -1;
    public int currentMessageBytesRead;
    private unsafe void OnReceived(long size)
    {
        int i = 0;
        while (i < size)
        {
            if (currentMessageSize == -1)
            {
                int amountToTake = Mathf.Min(4 - currentMessageBytesRead, (int) size - i);
                UnsafeUtility.MemCpy(messageBufferPtr + currentMessageBytesRead, receiveBufferPtr + i, amountToTake);
                currentMessageBytesRead += amountToTake;
                

                if (currentMessageBytesRead == 4)
                {
                    currentMessageBytesRead = 0;
                    currentMessageSize = messageBuffer.ReinterpretLoad<int>(0);
                }

                i += amountToTake;
            }
            else
            {
                //size-i gives us all the remaining bytes in this packet. If this is bigger than currentmessagesize - currentmessagebytesread (aka how many bytes left we want), then limit it to that.
                int amountToTake = Mathf.Min(currentMessageSize - currentMessageBytesRead, (int) size - i);
                UnsafeUtility.MemCpy(messageBufferPtr + currentMessageBytesRead, receiveBufferPtr + i, amountToTake);
                currentMessageBytesRead += amountToTake;

                if (currentMessageBytesRead == currentMessageSize)
                {
                    ProcessMessage();
                    currentMessageBytesRead = 0;
                    currentMessageSize = -1;
                }
            
                i += amountToTake;
            }
            
        }
    }

    private unsafe void ProcessMessage()
    {
        ServerMessage header = (ServerMessage)messageBuffer[0];
        switch (header)
        {
            case ServerMessage.CreateNetworkObject:
            {
                using (var stream = new UnmanagedMemoryStream(messageBufferPtr + 1, currentMessageSize - 1))
                {
                    NetworkObjectData networkObjectData = Serializer.Deserialize<NetworkObjectData>(stream);
                    NetworkObject networkObject = UnityEngine.Object.Instantiate(Networking.Resources.NetworkObjectPrefabsById[networkObjectData.Id], networkObjectData.Position, networkObjectData.Rotation);
                    networkObject.data = networkObjectData;
                    networkObjects.Add(networkObjectData.Id, networkObject);
                }
                break;
            }
            case ServerMessage.GameServerResponse:
            {
                uint requestId = messageBuffer.ReinterpretLoad<uint>(1);
                uint ip = messageBuffer.ReinterpretLoad<uint>(5);
                ushort port = messageBuffer.ReinterpretLoad<ushort>(9);

                if (requestDict.TryGetValue(requestId, out var callback))
                {
                    ((OnGameServerReceived) callback).Invoke(ip, port);
                }
                else
                {
                    Debug.LogError("Didn't find request id " + requestId);
                }
                
                break;
            }
            case ServerMessage.NetworkObjectResponse:
            {
                using (var stream = new UnmanagedMemoryStream(messageBufferPtr + 1, currentMessageSize - 1))
                {
                    IEnumerable<NetworkObjectData> list;
                    try
                    {
                        list = Serializer.DeserializeItems<NetworkObjectData>(stream, PrefixStyle.Base128, 1);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                    foreach(var networkObjectData in list)
                    {
                        var networkObject = GameObject.Instantiate(Items.WorldModelsMap[networkObjectData.ItemId], networkObjectData.Position, networkObjectData.Rotation);
                        networkObject.data = networkObjectData;
                        networkObjects.Add(networkObjectData.Id, networkObject);
                    }
                }
                break;
            }
            case ServerMessage.UpdateNetworkTransform:
            {
                using (var stream = new UnmanagedMemoryStream(messageBufferPtr + 1, currentMessageSize - 1))
                {
                    var transformPacket = Serializer.Deserialize<UpdateNetworkTransformPacket>(stream);
                }
                break;
            }
            default:
            {
                Debug.LogError("Invalid data header " + header + ".");
                break;
            }
        }
    }

    /// <summary>
    /// Tells the server to update a given transform
    /// </summary>
    /// <param name="t">Transform</param>
    /// <param name="id"></param>
    public void UpdateNetworkTransform(Transform t, Guid id)
    {
        UpdateNetworkTransformPacket packet = new()
        {
            Id = id,
            Position = t.position,
            Rotation = t.rotation
        };

        using (var stream = new MemoryStream())
        {
            stream.WriteByte((byte)ClientMessage.UpdateNetworkTransform);
            Serializer.Serialize(stream, packet);
            SendAsync(stream.ToArray());
        }
    }

    public delegate void OnGameServerReceived(uint ip, ushort port);
    
    public void RequestGameServer(OnGameServerReceived callback)
    {
        using (var stream = new MemoryStream())
        {
            stream.Seek(4, SeekOrigin.Begin);
            
            stream.WriteByte((byte)ClientMessage.RequestGameServer);
            stream.Write(BitConverter.GetBytes(RequestCount));
            requestDict.Add(RequestCount++, callback);
            
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((int)(stream.Length - 4)));
            
            SendAsync(stream.ToArray());
        }
    }

    public void RequestNetworkObjects()
    {
        using (var stream = new MemoryStream())
        {
            stream.Seek(4, SeekOrigin.Begin);
            
            stream.WriteByte((byte)ClientMessage.RequestNetworkObjects);
            
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((int)(stream.Length - 4)));
            
            SendAsync(stream.ToArray());
            
            Debug.Log("Sent request network objects.");
        }
    }

    protected override void OnError(SocketError error)
    {
        Debug.LogError(error);
    }
    
    public void OnApplicationQuit()
    {
        receiveBuffer.Dispose();
        messageBuffer.Dispose();
    }
}