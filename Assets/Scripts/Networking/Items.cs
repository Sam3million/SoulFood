using System.Collections.Generic;
using Networking;
using UnityEngine;
using Resources = UnityEngine.Resources;

public static class Items
{
    public static readonly Dictionary<ItemId, NetworkObject> WorldModelsMap = new ();
    
    static Items()
    {
        NetworkObject[] models = Resources.LoadAll<NetworkObject>("NetworkObjects");
        foreach (NetworkObject model in models)
        {
            WorldModelsMap.Add(model.data.ItemId, model);
        }
    }
}