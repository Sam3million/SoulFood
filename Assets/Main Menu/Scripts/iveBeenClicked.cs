using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iveBeenClicked : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip backgroundMusic;
    public AudioSource[] aSources;


    private void Start()
    {
        aSources = GetComponents<AudioSource>();
    }

    public void Clicked(string buttonName)
    {
        switch (buttonName)
        {
            case "Play":
                aSources[0].PlayOneShot(clickSound);
                // ADD WHAT YOU WANT THE
                // BUTTONS TO DO IN
                // THIS SCENARIO HERE
                break;
            case "Settings":
                aSources[0].PlayOneShot(clickSound);
                // ADD WHAT YOU WANT THE
                // BUTTONS TO DO IN
                // THIS SCENARIO HERE
                break;
        }
    }
}
