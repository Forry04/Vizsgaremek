using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        EquipSkinRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetInt("CurrentSkin"));

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
    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void EquipSkinRpc(ulong id, int skinId)
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Skindata skin = Resources.LoadAll<Skindata>("Skins").ToList().FirstOrDefault(s => s.skinId == skinId);

        Debug.Log($"Skin ID: {skinId}");

        foreach (var player in players)
        {
            if (!player.TryGetComponent<NetworkObject>(out var PlayerNetworkObJect)) continue;
            Debug.Log($"{PlayerNetworkObJect.OwnerClientId}");
            if (PlayerNetworkObJect.OwnerClientId == id)
            {
                Debug.Log($"Changing skin for {id}");
                player.GetComponentInChildren<Renderer>().material = skin.skinMaterial;

            }

        }

    }


}
