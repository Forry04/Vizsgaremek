using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawPlayerPostion : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        transform.SetPositionAndRotation(PlayerSpawner.Singleton.GetCurrentSpawnPoint().position, PlayerSpawner.Singleton.GetCurrentSpawnPoint().rotation);
        base.OnNetworkSpawn();
        Destroy(this);

    }

}
