using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SteamTest : MonoBehaviour
{
    // Our steam profile picture
    public RawImage profilePicture;
    // Our steam name
    public TMP_Text profileName;

    // Prefab that for player profile pciture and name
    public PlayerProfile profilePrefab;
    
    // A prefab of a lobby as it looks when it's in the lobby browser list 
    public LobbyListView lobbyListViewPrefab;
    
    // Gameobject that holds the lobby browser list
    public GameObject lobbyContainer;

    // Menu of when you are actually in a lobby
    public LobbyViewMenu lobbyViewMenu;

    public CreateLobbyMenu createLobbyMenu;
    
    private CallResult<LobbyMatchList_t> lobbyMatchListResult;
    private CallResult<LobbyEnter_t> joinLobbyResult;
    private CallResult<LobbyCreated_t> lobbyCreatedResult;
    private Callback<LobbyChatUpdate_t> lobbyChatUpdateCallback;
    
    void Start()
    {
        if (SteamManager.Initialized)
        {
            lobbyMatchListResult = CallResult<LobbyMatchList_t>.Create(OnRequestLobbyList);
            joinLobbyResult = CallResult<LobbyEnter_t>.Create(OnJoinLobby);
            lobbyCreatedResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);

            lobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            
            profileName.text = SteamFriends.GetPersonaName();
            CSteamID steamID = SteamUser.GetSteamID();

            profilePicture.texture = GetSteamProfilePicture(steamID, 2, out uint ImageWidth, out uint ImageHeight);
            profilePicture.rectTransform.sizeDelta = new Vector2(ImageWidth, ImageHeight);
        }
        else
        {
            Debug.LogError("Steam not connected. Make sure it is open.");
        }
    }

    public static Texture2D GetSteamProfilePicture(CSteamID steamID, int size, out uint ImageWidth, out uint ImageHeight)
    {
        int imagePtr;
        if (size == 2)
        {
            imagePtr = SteamFriends.GetLargeFriendAvatar(steamID);
        }
        else if (size == 1)
        {
            imagePtr = SteamFriends.GetMediumFriendAvatar(steamID);
        }
        else
        {
            imagePtr = SteamFriends.GetSmallFriendAvatar(steamID);
        }
        
        return FlipTexture(GetSteamImageAsTexture2D(imagePtr, out ImageWidth, out ImageHeight));
    }
    
    public static Texture2D GetSteamImageAsTexture2D(int iImage, out uint ImageWidth, out uint ImageHeight) {
        Texture2D ret = null;
        bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

        if (bIsValid) {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (bIsValid) {
                ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                ret.LoadRawTextureData(Image);
                ret.Apply();
            }
        }

        return ret;
    }
    
    static Texture2D FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);
     
        int xN = original.width;
        int yN = original.height;
     
        for(int i=0;i<xN;i++)
        {
            for(int j=0;j<yN;j++)
            {
                flipped.SetPixel(i, yN-j-1, original.GetPixel(i,j));
            }
        }

        flipped.Apply();
     
        return flipped;
    }

    public void CreateLobby()
    {
        int maxPlayers = int.Parse(createLobbyMenu.maxPlayersInputField.text);
        ELobbyType lobbyType = createLobbyMenu.friendsOnly.isOn ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePublic;
        lobbyCreatedResult.Set(SteamMatchmaking.CreateLobby(lobbyType, maxPlayers));
    }

    public void OnLobbyCreated(LobbyCreated_t data, bool bIsError)
    {
        if (data.m_eResult == EResult.k_EResultOK)
        {
            CSteamID lobbyId = new CSteamID(data.m_ulSteamIDLobby);
            SteamMatchmaking.SetLobbyData(lobbyId, "Name", createLobbyMenu.nameInputField.text);
            SteamMatchmaking.SetLobbyData(lobbyId, "SoulFoodLobby", "true");
            SetLobbyUI(lobbyId);
        }
        else
        {
            Debug.LogError("Failed to create lobby. Reason: " + data.m_eResult);
        }
    }

    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(lobbyViewMenu.lobbyId);
    }

    public void OnLobbyChatUpdate(LobbyChatUpdate_t data)
    {
        CSteamID lobbyMemberId = new CSteamID(data.m_ulSteamIDUserChanged);
        string lobbyMemberName = SteamFriends.GetFriendPersonaName(lobbyMemberId);

        CSteamID initiatorId = new CSteamID(data.m_ulSteamIDMakingChange);
        string initiatorName = SteamFriends.GetFriendPersonaName(initiatorId);

        CSteamID lobbyId = new CSteamID(data.m_ulSteamIDLobby);
        
        if (data.m_rgfChatMemberStateChange == 0x0001)
        {
            // joined or is joining
            AddLobbyMemberUI(lobbyId, lobbyMemberId);
            Debug.Log(lobbyMemberName + " joined the lobby.");
            
        }
        else if (data.m_rgfChatMemberStateChange == 0x0002)
        {
            // left or is leaving
            RemoveLobbyMemberUI(lobbyId, lobbyMemberId);
            Debug.Log(lobbyMemberName + " left the lobby.");
        }
        else if (data.m_rgfChatMemberStateChange == 0x0004)
        {
            // disconnected without first leaving the room
            RemoveLobbyMemberUI(lobbyId, lobbyMemberId);
            Debug.Log(lobbyMemberName + " disconnected.");
        }
        else if (data.m_rgfChatMemberStateChange == 0x0008)
        {
            // kicked
            RemoveLobbyMemberUI(lobbyId, lobbyMemberId);
            Debug.Log(lobbyMemberName + " was kicked from the lobby by " + initiatorName + ".");
        }
        else if (data.m_rgfChatMemberStateChange == 0x0010)
        {
            // kicked and blocked
            RemoveLobbyMemberUI(lobbyId, lobbyMemberId);
            Debug.Log(lobbyMemberName + " was banned from the lobby by " + initiatorName + ".");
        }
    }
    
    private void AddLobbyMemberUI(CSteamID lobbyId, CSteamID lobbyMemberId)
    {
        PlayerProfile lobbyMember = Instantiate(profilePrefab, lobbyViewMenu.playerContainer.transform);
        lobbyMember.profilePicture.texture = GetSteamProfilePicture(lobbyMemberId, 1, out uint ImageWidth, out uint ImageHeight);
        //lobbyMember.profilePicture.rectTransform.sizeDelta = new Vector2(ImageWidth, ImageHeight);
        lobbyMember.playerName.text = SteamFriends.GetFriendPersonaName(lobbyMemberId);
        lobbyMember.steamId = lobbyMemberId;
        lobbyViewMenu.playerCountFraction.text = SteamMatchmaking.GetNumLobbyMembers(lobbyId) + " / " + SteamMatchmaking.GetLobbyMemberLimit(lobbyId);
    }

    private void RemoveLobbyMemberUI(CSteamID lobbyId, CSteamID lobbyMemberId)
    {
        for(int i = 0; i < lobbyViewMenu.playerContainer.transform.childCount; i++)
        {
            if (lobbyViewMenu.playerContainer.transform.GetChild(i).GetComponent<PlayerProfile>().steamId ==
                lobbyMemberId)
            {
                Destroy(lobbyViewMenu.playerContainer.transform.GetChild(i).gameObject);
                lobbyViewMenu.playerCountFraction.text = SteamMatchmaking.GetNumLobbyMembers(lobbyId) + " / " + SteamMatchmaking.GetLobbyMemberLimit(lobbyId);
                break;
            }
        }
    }

    public void RequestLobbyList()
    {
        SteamMatchmaking.AddRequestLobbyListStringFilter("SoulFoodLobby", "true", ELobbyComparison.k_ELobbyComparisonEqual);
        lobbyMatchListResult.Set(SteamMatchmaking.RequestLobbyList());
    }

    private void GetRelevantLobbyData(CSteamID lobbyId, out string lobbyName, out int playerCount, out int maxPlayerCount)
    {
        playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
        maxPlayerCount = SteamMatchmaking.GetLobbyMemberLimit(lobbyId);
        lobbyName = SteamMatchmaking.GetLobbyData(lobbyId, "Name");
    }

    private void OnRequestLobbyList(LobbyMatchList_t data, bool bIsError)
    {
        for (int i = 0; i < data.m_nLobbiesMatching; i++)
        {
            var lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            GetRelevantLobbyData(lobbyId, out string lobbyName, out int playerCount, out int maxPlayerCount);

            var lobbyListView = Instantiate(lobbyListViewPrefab, lobbyContainer.transform);
            lobbyListView.lobbyName.text = lobbyName;
            lobbyListView.playerCountFraction.text = playerCount + " / " + maxPlayerCount;
            lobbyListView.lobbyId = lobbyId;
            lobbyListView.steamTest = this;
            
        }
        MainMenuManager.Instance.OpenMenu("LobbyBrowser");
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        joinLobbyResult.Set(SteamMatchmaking.JoinLobby(lobbyId));
    }

    public void OnJoinLobby(LobbyEnter_t data, bool bIsError)
    {
        if (data.m_EChatRoomEnterResponse == (uint) EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            CSteamID lobbyId = new CSteamID(data.m_ulSteamIDLobby);
            SetLobbyUI(lobbyId);
        }
        else
        {
            Debug.LogError("Failed to join lobby. Reason: " + (EChatRoomEnterResponse)data.m_EChatRoomEnterResponse);
        }
    }

    public void SetLobbyUI(CSteamID lobbyId)
    {
        GetRelevantLobbyData(lobbyId, out string lobbyName, out int playerCount, out int maxPlayerCount);
        lobbyViewMenu.lobbyId = lobbyId;
        lobbyViewMenu.lobbyName.text = lobbyName;
        for (int i = 0; i < playerCount; i++)
        {
            CSteamID lobbyMemberSteamId = SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i);
            AddLobbyMemberUI(lobbyId, lobbyMemberSteamId);
        }
            
        MainMenuManager.Instance.OpenMenu("Lobby");
    }
}
