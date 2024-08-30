using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HoverDetectionRaycast : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    public Light menuLight;
    public GameObject lastHit;
    public float lightIntensity;
    public bool hoveringEmptySpace;
    public bool hoveringButton;
    
    private Coroutine rotationCoroutine;
    private Coroutine intensityCoroutine;
    
    public bool rotationCoroutineRunning;
    public bool intensityCoroutineRunning;
    
    public float quaternionLerpTime;
    public float intensityLerpTime;
    
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,Mathf.Infinity))
        {
            hoveringEmptySpace = false;
            if (hoveringButton == false)
            {
                if (intensityCoroutineRunning)
                {
                    StopCoroutine(intensityCoroutine);
                }
                intensityCoroutineRunning = true;
                intensityCoroutine = StartCoroutine(LightIntensity(menuLight.intensity, lightIntensity, (1-(menuLight.intensity / lightIntensity)) * intensityLerpTime, 0));
                hoveringButton = true;
            }
            
            if (lastHit != hit.collider.gameObject)
            {
                if (rotationCoroutineRunning)
                {
                    StopCoroutine(rotationCoroutine);
                }
                rotationCoroutineRunning = true;
                
                Quaternion start = Quaternion.LookRotation(lastHit.transform.position - menuLight.transform.position);
                Quaternion end = Quaternion.LookRotation(hit.collider.transform.position - menuLight.transform.position);
                lastHit = hit.collider.gameObject;
                rotationCoroutine = StartCoroutine(LightRotation(start, end,quaternionLerpTime * QuaternionExtensions.InverseLerp(start,end, menuLight.transform.rotation), quaternionLerpTime));
            }
        }
        else
        {
            hoveringButton = false;
            if (hoveringEmptySpace == false)
            {
                if (intensityCoroutineRunning)
                {
                    StopCoroutine(intensityCoroutine);
                }
                intensityCoroutineRunning = true;
                intensityCoroutine = StartCoroutine(LightIntensity(menuLight.intensity, 0, (menuLight.intensity / lightIntensity) * intensityLerpTime,1));
                hoveringEmptySpace = true;
            }
        }
    }

    IEnumerator LightIntensity(float start, float end, float totalTime, float waitTime)
    {
        float time = 0;
        while (time < waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        while (time < totalTime)
        {
            menuLight.intensity = Mathf.Lerp(start,end,time/totalTime);
            time += Time.deltaTime;
            yield return null;
        }
        menuLight.intensity = end;
        intensityCoroutineRunning = false;
    }
    
    IEnumerator LightRotation(Quaternion start, Quaternion end, float time, float totalTime)
    {
        while (time < totalTime)
        {
            menuLight.transform.rotation = Quaternion.Lerp(start,end,time/totalTime);
            time += Time.deltaTime;
            yield return null;
        }
        menuLight.transform.rotation = end;
        rotationCoroutineRunning = false;
    }
}
