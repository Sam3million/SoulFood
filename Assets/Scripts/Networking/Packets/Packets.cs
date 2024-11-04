using System;
using ProtoBuf;
using UnityEngine;

namespace Networking.Packets
{
    // Use when sending message from client to server
    public enum ClientMessage : byte
    {
        UpdateNetworkTransform,
        UpdateNetworkTransformParentPacket,
    }
    
    // Use when sending message from server to client
    public enum ServerMessage : byte
    {
        CreateNetworkObject,
        UpdateNetworkTransform,
        
    }
    
    [ProtoContract]
    public struct UpdateNetworkTransformPacket
    {
        [ProtoMember(1)]
        public Guid Id;
        [ProtoMember(2)]
        public Vector3 Position;
        [ProtoMember(3)]
        public Quaternion Rotation;
    }
    
    [ProtoContract]
    public struct UpdateNetworkTransformParentPacket
    {
        [ProtoMember(1)]
        public Guid Id;
        [ProtoMember(2)]
        public Guid ParentId;
    }
    
    [ProtoContract]
    public struct CreateNetworkObjectPacket
    {
        public Guid Id;
    }
}