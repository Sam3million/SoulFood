using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyViewMenu : MonoBehaviour
{
    public CSteamID lobbyId;
    public TMP_Text lobbyName;
    // Text of the form: "playerCount / maxPlayerCount"  
    public TMP_Text playerCountFraction;
    public GridLayoutGroup playerContainer;
}