using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetCoreServer;
using Networking.Packets;
using Unity.Mathematics;
using UnityEngine;
using Buffer = System.Buffer;

public class Session : TcpSession
{
    public Session(TcpServer server) : base(server) {}

    protected override void OnConnected()
    {
        Debug.Log($"Client TCP session with Id {Id} connected!");

        // Send invite message
        //string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
        //SendAsync(message);
    }
    
    protected override void OnDisconnected()
    {
        Debug.Log($"Chat TCP session with Id {Id} disconnected!");

        (Server as Server).OnPlayerDisconnected(Id);
    }

    public byte[] messageBuffer = new byte[16384];
    public int currentMessageSize = -1;
    public int currentMessageBytesRead;

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        int i = 0;
        while (i < size)
        {
            if (currentMessageSize == -1)
            {
                int amountToTake = math.min(4 - currentMessageBytesRead, (int) size - i);
                Buffer.BlockCopy(buffer, i, messageBuffer, currentMessageBytesRead, amountToTake);
                currentMessageBytesRead += amountToTake;

                if (currentMessageBytesRead == 4)
                {
                    currentMessageBytesRead = 0;
                    currentMessageSize = BitConverter.ToInt32(messageBuffer, 0);
                }

                i += amountToTake;
            }
            else
            {
                //size-i gives us all the remaining bytes in this packet. If this is bigger than currentmessagesize - currentmessagebytesread (aka how many bytes left we want), then limit it to that.
                int amountToTake = math.min(currentMessageSize - currentMessageBytesRead, (int) size - i);
                Buffer.BlockCopy(buffer, i, messageBuffer, currentMessageBytesRead, amountToTake);
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

    private void ProcessMessage()
    {
        ClientMessage request = (ClientMessage)messageBuffer[0];
        switch (request)
        {
            case ClientMessage.UpdateNetworkTransform:
            {
                using (var stream = new MemoryStream(messageBuffer, 2 + 4, currentMessageSize - 2 - 4))
                {
                    try
                    {
                        var playerData = ProtoBuf.Serializer.Deserialize<PlayerData>(stream);
                        (Server as Server).UpdatePlayerData(Id, playerData);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        throw;
                    }
                }
                break;
            }
            default:
            {
                Debug.Log("Unknown request with id: " + messageBuffer[0]);
                break;
            }
        }
    }
}