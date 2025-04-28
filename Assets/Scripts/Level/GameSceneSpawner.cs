using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameSceneSpawner : MonoBehaviour
{
    [SerializeField] public List<Transform> spawnPoints;


    private void Start()
    {
      
        SpawnPlayersRpc(NetworkManager.Singleton.LocalClientId);

    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnPlayersRpc(ulong id)
    {
       SpawnPlayer(id);
    }






    private void SpawnPlayer(ulong id)
    {
        var player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id);
        if (player == null)
        {
            Debug.LogError("Player object not found for ID: " + id);
            return;
        }
        player.GetComponent<PlayerDie>().gameSceneSpawner = this;
        // Disable the character controller before repositioning
        player.GetComponent<CharacterController>().enabled = false;
        // Reposition the player to a spawn point
        player.transform.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        player.GetComponent<CharacterController>().enabled = true;



    }
}
