using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Menu3DButtons : MonoBehaviour
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent {}
    public ButtonClickedEvent OnClick = new ButtonClickedEvent();
}


