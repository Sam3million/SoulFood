using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using NetCoreServer;
using ProtoBuf;

public class Server : TcpServer
{
    protected readonly ConcurrentDictionary<Guid, Guid> steamIdBySessionId = new();
    protected readonly ConcurrentDictionary<Guid, PlayerData> players = new();
    public List<Vector3> playerPositions;
    
    public int maxPlayerCount;
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

    public void UpdatePlayerData(Guid id, PlayerData data)
    {
        data.lastPosition = players[steamIdBySessionId[id]].lastPosition;
        players[steamIdBySessionId[id]] = data;
    }

    public bool SetId(Guid sessionId, Guid steamId)
    {
        return steamIdBySessionId.TryAdd(sessionId, steamId);
    }
    
    public void OnPlayerDisconnected(Guid disconnectId)
    {
        steamIdBySessionId.TryRemove(disconnectId, out var steamId);
        players.TryRemove(steamId, out var playerData);
    }
}
