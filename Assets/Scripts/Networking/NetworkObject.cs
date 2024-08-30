using System;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class NetworkObject : MonoBehaviour
    {
        [SerializeReference, SubclassSelector]
        public NetworkObjectData data;
    }
    
    [ProtoContract]
    [ProtoInclude(10, typeof(NetworkFruit))]
    // add another protoinclude for all derived classes
    [Serializable]
    public class NetworkObjectData
    {
        [ProtoMember(1)]
        public Guid Id;
        [ProtoMember(2)]
        public Vector3 Position;
        [ProtoMember(3)] 
        public Quaternion Rotation;
    }

    [ProtoContract]
    [Serializable]
    public class NetworkFruit : NetworkObjectData
    {
        [ProtoMember(1)]
        public bool isApple;
    }
}