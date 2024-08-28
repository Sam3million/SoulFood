using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class clickDetectionRaycast : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Input.GetMouseButtonDown(0))
        {
            string sendName;
            switch (hit.collider.gameObject.name)
            {
                case "playcan":
                    sendName = "Play";
                    break;
                
                case "cerealbox":
                    sendName = "Settings";
                    break;
                
                default:
                    sendName = null;
                    break;
            }
            GetComponent<iveBeenClicked>().Clicked(sendName);
        }
    }
}
