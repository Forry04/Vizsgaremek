using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] playerSpawnPoint;
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private int spawnPostionIndex = 0;

    public static PlayerSpawner Singleton { get; private set; }

    private void Start()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;



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
        if (IsServer && IsHost) StartCoroutine(WaitForClientConnected(NetworkManager.Singleton.LocalClientId));
        base.OnNetworkSpawn();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer) StartCoroutine(WaitForClientConnected(clientId));
    }

    private IEnumerator WaitForClientConnected(ulong clientId)
    {
        while (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            yield return new WaitForSeconds(0.1f);
        }
        SpawnPlayerServer(clientId);
    }

    
    private void SpawnPlayerServer(ulong clientId)
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint[spawnPostionIndex].position, playerSpawnPoint[spawnPostionIndex].rotation);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        spawnPostionIndex++;
    }
    public Transform GetCurrentSpawnPoint() => playerSpawnPoint[spawnPostionIndex];


}
