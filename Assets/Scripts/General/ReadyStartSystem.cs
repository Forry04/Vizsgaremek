using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public class ReadyStartSystem : NetworkBehaviour
{

    List<PlayerReady> PlayerReadyList;

   static  public ReadyStartSystem Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    public override void OnNetworkSpawn()
    {
       

        if (!IsServer && IsHost) return;
        PlayerReadyList = new()
        {
            new(NetworkManager.Singleton.LocalClientId)
        };
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

       
        base.OnNetworkSpawn();
    }

    public override void OnDestroy()
    {
        
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        base.OnDestroy();
    }

    private void OnClientConnected(ulong clientId)
    {
        PlayerReadyList.Add(new PlayerReady(clientId));
    }

    public void SetPlayerState(ulong id)
    {
        SetPlayerStateRpc(id);

    }




    [Rpc(SendTo.Server,RequireOwnership = false)]
    private void SetPlayerStateRpc(ulong id)
    {
        var player = PlayerReadyList.Find(p => p.playerID == id);
        if (player.Equals(default)) return;
        player.isReady = !player.isReady;

        Chat.Singleton.SendAnnouncement($"{PlayerDataManager.Singleton.Name} is {(!player.isReady ? "not" : "")} ready!",(player.isReady ? Color.green : Color.red));
    }

  


}

internal class PlayerReady
{
    public ulong playerID;
    public bool isReady;
    public PlayerReady(ulong playerID, bool isReady = false)
    {
        this.playerID = playerID;
        this.isReady = isReady;
    }
   
}
