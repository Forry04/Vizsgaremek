using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class CoinsManager : NetworkBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField]
    GameObject[] coins = new GameObject[7];
    List<Coin> coinsList = new List<Coin>();

    private TimeSpan gameTime;

    private void Start()
    {
        coinsList = coins.Select(coin => coin.GetComponent<Coin>()).ToList();
        gameTime = TimeSpan.FromSeconds(0);
    }
    private void Update()
    {
        if (coinsList.All(c=>c.colllected))
        {
            EndGameRpc();
        }

    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void EndGameRpc()
    {
        winScreen.SetActive(true);
        gameTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));



        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        SceneTransitionManager.Instance.TransitionToMenu();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);


    }




}
