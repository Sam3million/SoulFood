using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickDetectionRaycast : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Input.GetMouseButtonDown(0))
        {
            hit.collider.GetComponent<Menu3DButtons>().OnClick.Invoke();
        }
    }
}