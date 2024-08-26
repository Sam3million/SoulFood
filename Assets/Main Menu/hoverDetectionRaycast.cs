using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoverDetectionRaycast : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    public Light menuLight;
    private Coroutine coroutine;
    public GameObject lastHit;
    public bool coroutineRunning;
    public float lerpTime;
    
    
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,Mathf.Infinity))
        {
            //
            //menuLight.transform.LookAt(hit.collider.transform.position);
            if (lastHit != hit.collider.gameObject)
            {
                if (coroutineRunning)
                {
                    StopCoroutine(coroutine);
                }

                coroutineRunning = true;
                Quaternion start = Quaternion.LookRotation(lastHit.transform.position - menuLight.transform.position);
                Quaternion end = Quaternion.LookRotation(hit.collider.transform.position - menuLight.transform.position);
                lastHit = hit.collider.gameObject;
                coroutine = StartCoroutine(Bobi(start, end,lerpTime * QuaternionExtensions.InverseLerp(start,end, menuLight.transform.rotation), lerpTime));
                print("launched coroutine");
            }
        }
    }
    
    // FUCK FUCK FUCK
    IEnumerator Bobi(Quaternion start, Quaternion end, float time, float totalTime)
    {
        while (time < totalTime)
        {
            menuLight.transform.rotation = Quaternion.Lerp(start,end,time/totalTime);
            time += Time.deltaTime;
            yield return null;
        }
        menuLight.transform.rotation = end;
        coroutineRunning = false;
    }
}
