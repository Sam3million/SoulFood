using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using NetCoreServer;
using Networking;
using ProtoBuf;
using ProtoBuf.Meta;

public class Server : TcpServer
{
    protected readonly ConcurrentDictionary<Guid, Guid> steamIdBySessionId = new();
    protected readonly ConcurrentDictionary<Guid, NetworkObjectData> networkObjects = new();
    
    public string persistentDataPath;

    public Server(IPAddress address, int port) : base(address, port) { }

    public Server(string address, int port) : base(address, port)
    {
    }

    public Server(DnsEndPoint endpoint) : base(endpoint) { }

    public Server(IPEndPoint endpoint) : base(endpoint) { }
    
    protected override TcpSession CreateSession() { return new Session(this); }
    
    public void Tick()
    {
        
    }

    /// <summary>
    /// Adds already instantiated network object to the networkobjects dictionary.
    /// </summary>
    /// <param name="networkObject"></param>
    public void AddNetworkObject(NetworkObjectData networkObjectData)
    {
        networkObjects.TryAdd(networkObjectData.Id, networkObjectData);
    }
    
    public void UpdateNetworkObject(Guid id, NetworkObjectData data)
    {
        if(networkObjects.ContainsKey(id))
        {
            networkObjects[id] = data;
        }
        else
        {
            Debug.LogError("Failed to find network object id " + id + ".");
        }
    }

    public bool SetId(Guid sessionId, Guid steamId)
    {
        return steamIdBySessionId.TryAdd(sessionId, steamId);
    }
    
    public void OnPlayerDisconnected(Guid disconnectId)
    {
        steamIdBySessionId.TryRemove(disconnectId, out var steamId);
        //players.TryRemove(steamId, out var playerData);
    }

    public IEnumerable<NetworkObjectData> GetNetworkObjects()
    {
        return networkObjects.Values;
    }
}
