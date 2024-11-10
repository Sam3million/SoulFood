using System;
using Networking;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        if (Client.Instance != null)
        {
            // destroy them client side because they need to be spawned by the server
            var NetworkObjects = FindObjectsByType<NetworkObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var networkObject in NetworkObjects)
            {
                Destroy(networkObject.gameObject);
            }

            Client.Instance.RequestNetworkObjects();
        }
    }
}
