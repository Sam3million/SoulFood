using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net;
using ProtoBuf.Meta;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    /// <summary>
    /// Controls the game world as it appears on the server
    /// </summary>
    public class ServerGameManager : MonoBehaviour
    {
        private Server server;
        public ServerConsole console;

        public void Awake()
        {
            RuntimeTypeModel.Default.Add(typeof(Vector3), false).Add("x", "y", "z");
            RuntimeTypeModel.Default.Add(typeof(Quaternion), false).Add("x", "y", "z", "w");
            
            console.AwakeManual(OnInputText);
            
            DontDestroyOnLoad(this);
            
            var commandLineArgs = System.Environment.GetCommandLineArgs();

            // Uncomment once matchmaking server gets made
            /*string ip = commandLineArgs[1];
            int port = int.Parse(commandLineArgs[2]);
            string level = commandLineArgs[3];
            string pipeName = commandLineArgs[4];*/
            
            string ip = "127.0.0.1";
            int port = 1937;
            string level = "Grocery";

            StartCoroutine(LoadSceneAsync(ip, port, level));
        }

        IEnumerator LoadSceneAsync(string ip, int port, string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncOperation.isDone)
            {
                yield return null; 
            }
            
            // IP and Port are command line args specified when the exe was launched by the matchmaking server
            server = new Server(ip, port);
            if (!server.Start())
            {
                Debug.Log("Failed to start server.");
            }
            else
            {
                Debug.Log("Started server on level " + sceneName + "!");
                Debug.Log("Server IP: " + server.Address);
                Debug.Log("Server status: " + (server.IsAccepting ? "Accepting" : "Not Accepting"));
            }
            
            // Initializing network objects with guids and place them in dictionary:
            var networkObjectsArray = FindObjectsByType<NetworkObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var networkObject in networkObjectsArray)
            {
                networkObject.data.Id = Guid.NewGuid();
                networkObject.data.Position = networkObject.transform.position;
                networkObject.data.Rotation = networkObject.transform.rotation;
                
                server.AddNetworkObject(networkObject.data);
            }
            
            // Send message to matchmaking server that we are ready to receive players.
            // It will then give the game server ip and port to the lobby members.
            /*NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
            pipeClient.Connect();

            pipeClient.WriteByte(server.IsAccepting ? (byte)1 : (byte)0);*/
        }
        
        /// <summary>
        /// Text has been entered into the console
        /// Run it as a console command
        /// </summary>
        /// <param name="line"></param>
        void OnInputText(string line)
        {
            if (line == "stop")
            {
                if (server.IsStarted)
                {
                    Debug.Log("Server stopping...");
                    server.Stop();
                    Debug.Log("Done!");
                    Application.Quit();
                }
            }
            else if (line == "restart")
            {
                if (server.IsStarted)
                {
                    Debug.Log("Server restarting...");
                    if (server.Restart())
                    {
                        Debug.Log("Server successfully restarted!");
                    }
                    else
                    {
                        Debug.Log("Server failed to restart...");
                    }
                }
                else
                {
                    Debug.Log("Server is not started...");
                }
            }
        }

        public void FixedUpdate()
        {
            if (server != null && server.IsStarted)
            {
                server.Tick();
            }
        }
    }
}