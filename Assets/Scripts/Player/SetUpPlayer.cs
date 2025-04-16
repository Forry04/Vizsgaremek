using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SetUpPlayer : NetworkBehaviour
{
    CharacterController characterController;

    [SerializeField] GameObject playerNameTag;

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
        }
        characterController.enabled = true;

        base.OnNetworkSpawn();
        Destroy(this);
    }

    

    

}
