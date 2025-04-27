using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour, ICollectible
{
    public bool colllected = false;
    public void Collect()
    {
        CollectServerRpc();
        colllected = true;
        gameObject.SetActive(false);
    }

    [Rpc(SendTo.Everyone,RequireOwnership =false)]
    public void CollectServerRpc()
    {
        PlayerDataManager.Singleton.Coins ++;
    }




}
