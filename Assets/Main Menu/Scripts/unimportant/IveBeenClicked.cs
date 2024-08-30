using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundHandler : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource[] aSources;
    
    private void Start()
    {
        aSources = GetComponents<AudioSource>();
    }

    public void Clicked()
    {
        aSources[0].PlayOneShot(clickSound);
    }
}
