using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class CoinsManager : NetworkBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject[] coins = new GameObject[7];
    List<Coin> coinsList = new List<Coin>();

    private TimeSpan gameTime;

    private void Start()
    {
        coinsList = coins.Select(coin => coin.GetComponent<Coin>()).ToList();
        gameTime = TimeSpan.FromSeconds(0);
    }

    private void Update()
    {
        if (coinsList.All(c => c.colllected))
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
        ChangeCoins(7);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        SceneTransitionManager.Instance.TransitionToMenu();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private async void ChangeCoins(int amount)
    {
        string apiUrl = "https://api.arachnid-descent.games/api/user/change-balance"; // Replace with your API URL
        string token = PlayerPrefs.GetString("token"); // Retrieve the token from PlayerPrefs

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Token is missing. Cannot change coins.");
            return;
        }

        var requestBody = new
        {
            amount = amount // The amount to change the balance
        };

        using (var client = new HttpClient())
        {
            // Add authentication header
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Serialize the request body to JSON
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send POST request
                var response = await client.PostAsync(apiUrl, content);

                // Read and log the response
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.Log($"Status Code: {response.StatusCode}");
                Debug.Log($"Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Coin balance updated successfully.");
                }
                else
                {
                    Debug.LogError("Failed to update coin balance.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurred while changing coins: {ex.Message}");
            }
        }
    }
}
