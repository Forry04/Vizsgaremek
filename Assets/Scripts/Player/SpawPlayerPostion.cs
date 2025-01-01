using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawPlayerPostion : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            transform.SetPositionAndRotation(PlayerSpawner.Singleton.GetCurrentSpawnPoint().position, PlayerSpawner.Singleton.GetCurrentSpawnPoint().rotation);
            Destroy(this);
        }
            base.OnNetworkSpawn();
    }

}
