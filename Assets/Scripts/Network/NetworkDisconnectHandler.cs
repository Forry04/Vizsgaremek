using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkDisconnectHandler : MonoBehaviour
{
    private ulong serverClientId;
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
       
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        Debug.LogWarning($"Client disconnected: {clientId}");
        Debug.LogWarning($"Server Client ID: {serverClientId}");

        if (clientId == 1)
        {
            Debug.LogWarning("Host disconnected. Returning to main menu...");

            StartCoroutine(ShutdownAndLoadMenu());
        }
    }

    private IEnumerator ShutdownAndLoadMenu()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }

        yield return null; // Wait for one frame to ensure shutdown completes
        SceneManager.LoadScene("Menu", LoadSceneMode.Single); // Replace "Menu" with your actual main menu scene name
        this.enabled = false;
    }
}
