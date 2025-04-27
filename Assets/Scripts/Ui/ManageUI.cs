using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
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
                // Add the token to the Authorization header
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                // Send a GET request to validate the token
                HttpResponseMessage response = await client.GetAsync($"{PlayerDataManager.BaseApiUrl}/auth/check-token");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Token is valid: " + responseBody);

                    // Token is valid, show the main menu
                    MainMenuUiObject.SetActive(true);
                    GameManager.Singleton.gameObject.GetComponent<LoadSkins>().Loadskins();
                    
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
}
