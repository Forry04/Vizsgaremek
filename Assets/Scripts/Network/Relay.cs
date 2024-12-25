using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using System.Net.NetworkInformation;


/// <summary>
/// Manages network connections using Unity Relay and local network for offline mode.
/// </summary>
public class Relay : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Relay class.
    /// </summary>
    public static Relay Singleton { get; private set; }

    /// <summary>
    /// The join code for connecting to a relay server.
    /// </summary>
    public string JoinCode { get; private set; }

    /// <summary>
    /// Indicates whether the application is running in offline mode.
    /// </summary>
    public bool IsOfflineMode { get; private set; }

    private UnityTransport unityTransport;

    /// <summary>
    /// Initializes the Relay instance and sets up network services.
    /// </summary>
    private async void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);

       

        if (IsInternetAvailable())
        {
            await InitializeServicesAsync();
            IsOfflineMode = false;
        }
        else
        {
            Debug.LogWarning("No internet connection. Running in offline mode.");
            IsOfflineMode = true;
        }
    }
    private void Start()
    {
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    /// <summary>
    /// Initializes Unity services and signs in anonymously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize services: {e.Message}");
            IsOfflineMode = true;
        }
    }

    /// <summary>
    /// Creates a relay server or starts a local host if offline.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains
    /// true if the relay server or local host was successfully created; otherwise, false.
    /// </returns>
    /// <exception cref="RelayServiceException">Thrown when there is an error with the relay service.</exception>
    public async Task<bool> CreateRelay()
    {
        if (IsOfflineMode)
        {
            Debug.LogWarning("Cannot create relay in offline mode. Starting local host.");
            JoinCode = "Offline";
            StartLocalHost();
            return true;
        }

        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(3);
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            GUIUtility.systemCopyBuffer = JoinCode;
            return true;
        }
        catch (RelayServiceException e)
        {
            HandleRelayException(e);
            return false;
        }
    }

    /// <summary>
    /// Joins a relay server or starts a local client if offline.
    /// </summary>
    /// <param name="joinCode">The join code for the relay server.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains
    /// true if successfully joined the relay server or started the local client; otherwise, false.
    /// </returns>
    /// <exception cref="RelayServiceException">Thrown when there is an error with the relay service.</exception>
    public async Task<bool> JoinRelay(string joinCode)
    {
        if (IsOfflineMode)
        {
            Debug.LogWarning("Cannot join relay in offline mode. Starting local client.");
            StartLocalClient();
            return true;
        }

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var relayServerData = new RelayServerData(joinAllocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            return true;
        }
        catch (RelayServiceException e)
        {
            HandleRelayException(e);
            return false;
        }
    }

    /// <summary>
    /// Starts a local host for offline mode.
    /// </summary>
    private void StartLocalHost()
    {
        unityTransport.SetConnectionData("127.0.0.1", 7777, "0.0.0.0");
        NetworkManager.Singleton.StartHost();
    }

    /// <summary>
    /// Starts a local client for offline mode.
    /// </summary>
    private void StartLocalClient()
    {
        unityTransport.SetConnectionData("127.0.0.1", 7777, "0.0.0.0");
        NetworkManager.Singleton.StartClient();
    }

    /// <summary>
    /// Handles exceptions thrown by the relay service.
    /// </summary>
    /// <param name="e">The exception thrown by the relay service.</param>
    private void HandleRelayException(RelayServiceException e)
    {
        NetworkManager.Singleton.Shutdown();
        Debug.LogError($"RelayServiceException: {e.Message}");
    }

    /// <summary>
    /// Checks if the internet is available.
    /// </summary>
    /// <returns>True if the internet is available; otherwise, false.</returns>
    private bool IsInternetAvailable()
    {
        try
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to check internet availability: {e.Message}");
            return false;
        }
    }
}
