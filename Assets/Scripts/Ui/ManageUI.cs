using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

public class ManageUI : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUiObject;
    [SerializeField] private GameObject LoginMenuUiObject;

    private void Start()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("token")))
        {
            CheckToken();
        }
        else
        {
            LoginMenuUiObject.SetActive(true);
        }
    }

    private async void CheckToken()
    {
        string token = PlayerPrefs.GetString("token");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Token is missing");
            LoginMenuUiObject.SetActive(true);
            return;
        }

        using (HttpClient client = new HttpClient())
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"{PlayerDataManager.BaseApiUrl}/auth/check-token");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Token is valid: " + responseBody);

                    MainMenuUiObject.SetActive(true);
                    GameManager.Singleton.gameObject.GetComponent<LoadSkins>().Loadskins();

                    // Fetch the username
                    await GetUserName(token);
                }
                else
                {
                    Debug.LogError("Invalid or expired token: " + response.StatusCode);
                    LoginMenuUiObject.SetActive(true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occurred while validating token: " + ex.Message);
                LoginMenuUiObject.SetActive(true);
            }
        }
    }

    public async Task GetUserName(string token)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Add the Authorization header with the Bearer token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send the GET request
                HttpResponseMessage response = await client.GetAsync($"{PlayerDataManager.BaseApiUrl}/user/details");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Raw JSON Response: " + responseBody);

                    // Parse the response to extract the username and balance
                    var userProfile = JsonUtility.FromJson<UserProfile>(responseBody);
                    Debug.Log($"Welcome, {userProfile.username}! Your balance is {userProfile.balance}.");
                    PlayerPrefs.SetString("username", userProfile.username);
                }
                else
                {
                    Debug.LogError("Failed to fetch user profile: " + response.StatusCode);
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error details: " + errorResponse);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occurred while fetching user profile: " + ex.Message);
            }
        }
    }

    [Serializable]
    private class UserProfile
    {
        public string username;
        public int balance;
    }
}
