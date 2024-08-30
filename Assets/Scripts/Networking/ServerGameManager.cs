using System;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// Controls the game world as it appears on the server
    /// </summary>
    public class ServerGameManager : MonoBehaviour
    {
        // TODO: Uncomment once Julian creates Server class
        //private Server server;
        private Dictionary<Guid, NetworkObject> networkObjects;

        public void Awake()
        {
            // Initializing network objects with guids and place them in dictionary:
            var networkObjectsArray = FindObjectsByType<NetworkObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var networkObject in networkObjectsArray)
            {
                networkObject.data.Id = Guid.NewGuid();
                networkObject.data.Position = networkObject.transform.position;
                networkObject.data.Rotation = networkObject.transform.rotation;
                
                networkObjects.Add(networkObject.data.Id, networkObject);
            }
            
            /*server = new Server(ip, port);
            if (!server.Start())
            {
                Debug.Log("Failed to start server.");
            }
            else
            {
                Debug.Log("Started server!");
                Debug.Log(server.Address);
                Debug.Log(server.IsStarted + " " + server.IsAccepting);
            }*/
        }

        public void FixedUpdate()
        {
            //server.Tick();
        }
    }
}