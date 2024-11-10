using System;
using System.Net;
using ProtoBuf.Meta;
using UnityEngine;

public class ClientWrapper : MonoBehaviour
{
    private void Start()
    {
        // Connecting to matchmaking server:
        if (Client.Instance == null)
        {
            RuntimeTypeModel.Default.Add(typeof(Vector3), false).Add("x", "y", "z");
            RuntimeTypeModel.Default.Add(typeof(Quaternion), false).Add("x", "y", "z", "w");
            
            Client.Instance = new Client(IPAddress.Loopback, 1937);
            if (Client.Instance.Connect())
            {
                Debug.Log("Connected to matchmaker.");
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogError("Failed to connect to matchmaking server.");
            }
        }
    }

    private void Update()
    {
        Client.Instance.Receive();
    }
}