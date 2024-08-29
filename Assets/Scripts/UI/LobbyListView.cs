using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyListView : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TMP_Text lobbyName;
    // Text of the form: "playerCount / maxPlayerCount"  
    public TMP_Text playerCountFraction;
    public CSteamID lobbyId;
    public RawImage background;
    
    [NonSerialized]
    public SteamTest steamTest;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0.25f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        steamTest.JoinLobby(lobbyId);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0.1f);
    }
}