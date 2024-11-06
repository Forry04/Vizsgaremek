using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private  Transform[] playerSpawnPoint;
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private void Start()
    {
        if (IsServer) NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        if (IsHost) joinCodeText.text = Relay.Singleton.JoinCode;

    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null && IsServer) NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        base.OnDestroy();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && IsHost) SpawnPlayerServer(NetworkManager.Singleton.LocalClientId);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer) SpawnPlayerServer(clientId);
        
        
    }

    private void SpawnPlayerServer(ulong clientId)
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint[0].position, playerSpawnPoint[0].rotation);
        NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();
        playerNetworkObject.SpawnAsPlayerObject(clientId);
    }
}