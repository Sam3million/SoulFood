using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void SetMasterLevel(float level)
    {
        float db = 20 * Mathf.Log10(level);
        masterMixer.SetFloat("Master", db);
    }
    
    public void SetBackgroundLevel(float level)
    {
        float db = 20 * Mathf.Log10(level);
        masterMixer.SetFloat("Background", db);
    }

    public void SetInterfaceLevel(float level)
    {
        float db = 20 * Mathf.Log10(level);
        masterMixer.SetFloat("Interface", db);
    }
}
