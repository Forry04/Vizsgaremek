using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SpawPlayerPostion : NetworkBehaviour
{
    CharacterController characterController;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
       
        if (IsServer)
        {
            transform.SetPositionAndRotation(PlayerSpawner.Singleton.PlayerSpawnPoint.position, PlayerSpawner.Singleton.PlayerSpawnPoint.rotation);
            characterController.enabled = true;
        }
        Destroy(this);
        base.OnNetworkSpawn();
    }

    

}
