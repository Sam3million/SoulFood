using System;
using System.Collections.Generic;

namespace Networking
{
    public static class Resources
    {
        public static readonly Dictionary<Guid, NetworkObject> NetworkObjectPrefabsById = new();

        static Resources()
        {
            NetworkObject[] networkObjectPrefabs = UnityEngine.Resources.LoadAll<NetworkObject>("NetworkObjects");
            foreach (var networkObject in networkObjectPrefabs)
            {
                NetworkObjectPrefabsById.Add(networkObject.data.Id, networkObject);
            }
        }
    }
}