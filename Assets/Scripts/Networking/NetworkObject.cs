using System;
using System.IO;
using Networking.Packets;
using ProtoBuf;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class NetworkObject : MonoBehaviour
    {
        [SerializeReference, SubclassSelector]
        public NetworkObjectData data;

        public void SetParent(NetworkObject parent, Client client)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                UpdateNetworkTransformParentPacket packet = new()
                {
                    Id = data.Id,
                    ParentId = parent.data.Id
                };
                
                stream.Seek(4, SeekOrigin.Begin);
                
                stream.WriteByte((byte)ClientMessage.UpdateNetworkTransformParentPacket);
                ProtoBuf.Serializer.Serialize(stream, packet);
                
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(BitConverter.GetBytes((int)stream.Length - 4));

                client.Send(stream.ToArray());
            }
        }
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