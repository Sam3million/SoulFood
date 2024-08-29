using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image buttonImage;
    public bool selectedOnStart;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (selectedOnStart)
        {
            tabGroup.OnTabPressed(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabHover(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabPressed(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
}
