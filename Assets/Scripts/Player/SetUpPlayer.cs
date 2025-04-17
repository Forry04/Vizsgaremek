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
    TextMeshProUGUI playerNameText;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        playerNameText = playerNameTag.GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            transform.SetPositionAndRotation(PlayerSpawner.Singleton.PlayerSpawnPoint.position, PlayerSpawner.Singleton.PlayerSpawnPoint.rotation);
        }
        characterController.enabled = true;

        SetPlayerNameTagRpc(NetworkManager.Singleton.LocalClientId, PlayerDataManager.Singleton.Name);

        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetPlayerNameTagRpc(ulong id,string name)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.TryGetComponent<NetworkObject>(out NetworkObject networkObject)) continue;

            if (networkObject.OwnerClientId == id)
            {
                networkObject.GetComponent<SetUpPlayer>().playerNameText.text = name;
            }

        }

    }
}
