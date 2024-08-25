using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamTest : MonoBehaviour
{
    public RawImage profilePicture;
    public TMP_Text profileName;
    
    void Start()
    {
        if (SteamManager.Initialized)
        {
            profileName.text = SteamFriends.GetPersonaName();
            CSteamID steamID = SteamUser.GetSteamID();
            int imagePtr = SteamFriends.GetLargeFriendAvatar(steamID);
            profilePicture.texture = FlipTexture(GetSteamImageAsTexture2D(imagePtr, out uint ImageWidth, out uint ImageHeight));
            profilePicture.rectTransform.sizeDelta = new Vector2(ImageWidth, ImageHeight);
        }
        else
        {
            Debug.LogError("Steam not connected. Make sure it is open.");
        }
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
}
