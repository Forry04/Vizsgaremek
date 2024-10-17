using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPlayerServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnPlayerServerRpc()
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();
        playerNetworkObject.SpawnWithOwnership(OwnerClientId);
        SetupPlayerCameraClientRpc(playerNetworkObject.OwnerClientId);
    }

    [ClientRpc]
    private void SetupPlayerCameraClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            Debug.Log("!Player!");
            Debug.Log(player);



            //Camera playerCamera = player.GetComponentInChildren<Camera>();
            //if (playerCamera != null)
            //{
            //    playerCamera.enabled = true;
            //    playerCamera.tag = "MainCamera";
            //}
        }
    }
}
