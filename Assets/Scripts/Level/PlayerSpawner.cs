using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] playerSpawnPoint;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    private int spawnPostionIndex;


    public Transform PlayerSpawnPoint
    {
        get
        {
            Transform spawnPoint = playerSpawnPoint[spawnPostionIndex];
            spawnPostionIndex = (spawnPostionIndex + 1) % playerSpawnPoint.Length;
            return spawnPoint;
        }
    }

    public static PlayerSpawner Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }


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
        base.OnNetworkSpawn();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer) SpawnPlayerServer(clientId);
    }

    private void SpawnPlayerServer(ulong clientId)
    {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

  
    
       
}

