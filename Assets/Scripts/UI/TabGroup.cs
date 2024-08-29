using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabs = new List<TabButton>();
    private TabButton selectedTab;
    private TabButton hoverTab;
    public List<GameObject> objectsToSwap = new();

    private void Awake()
    {
        if (tabs.Count > 0)
        {
            OnTabPressed(tabs[0]);
        }
    }

    public void OnTabPressed(TabButton tab)
    {
        selectedTab = tab;
        UpdateTabs();
        int index = tab.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            objectsToSwap[i].SetActive(i == index);
        }
    }

    public void OnTabHover(TabButton tab)
    {
        hoverTab = tab;
        UpdateTabs();
    }
    
    public void OnTabExit(TabButton tab)
    {
        hoverTab = null;
        UpdateTabs();
    }

    void UpdateTabs()
    {
        foreach (TabButton tab in tabs)
        {
            if (tab == selectedTab)
            {
                tab.buttonImage.color = new Color(tab.buttonImage.color.r, tab.buttonImage.color.g, tab.buttonImage.color.b, 0.2f);
            }
            else if (tab == hoverTab)
            {
                tab.buttonImage.color = new Color(tab.buttonImage.color.r, tab.buttonImage.color.g, tab.buttonImage.color.b, 0.1f);
            }
            else
            {
                tab.buttonImage.color = new Color(tab.buttonImage.color.r, tab.buttonImage.color.g, tab.buttonImage.color.b, 0f);
            }
        }
    }
}
