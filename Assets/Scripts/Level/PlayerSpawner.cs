using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    // if lobby is loadead spawn players
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}

    