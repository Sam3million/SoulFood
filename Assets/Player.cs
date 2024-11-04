using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Networking;
using Steamworks;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public CharacterController characterController;
    public float movementSpeed = 5;
    public float pickupDist = .5f;
    public float dropDist = .5f;

    public GameObject itemHeld = null;
    private Camera cam;
    private GameObject selected;

    public Client client;
    
    private void Start()
    {
        cam = Camera.main;
        client = new Client(IPAddress.Loopback, 1912);
        client.Connect();
    }

    void Update()
    {
        client.Receive();
        
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }
        characterController.Move(moveDir.normalized * (Time.deltaTime * movementSpeed));

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            
            
            // Item pickup
            if (Input.GetMouseButtonDown(0))
            {
                selected = hit.collider.gameObject;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (selected != null)
                {
                    if ((hit.transform == selected.transform) && (hit.transform.CompareTag("item")) && Vector3.Distance(hit.point, transform.position) < pickupDist && itemHeld == null )
                    {
                        selected.transform.SetParent(transform.parent);
                        selected.GetComponent<NetworkObject>().SetParent(transform.parent.GetComponent<NetworkObject>(), client);
                        itemHeld = selected;
                        selected.transform.localPosition = new Vector3(0, 1.5f, 0);

                        selected = null;
                    }
                    else
                    {
                        if (hit.transform != null && Vector3.Distance(hit.point, transform.position) < dropDist)
                        {
                            if (selected.transform.CompareTag("deposit") && itemHeld != null)
                            {
                                itemHeld.transform.SetParent(selected.transform);
                                itemHeld.transform.position = hit.point;
                                selected.gameObject.GetComponent<Checkout>().addItem(itemHeld);
                                itemHeld = null;
                            }
                        }

                    }
                }


            }
        }
        
    }
}
