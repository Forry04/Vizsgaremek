using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public class Relay : MonoBehaviour
{

    public static Relay Singleton { get; private set; }

    public string JoinCode;

    private async void Awake()
    {
        if (Singleton!=null&& Singleton!=this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);

        }

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task<bool> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new(allocation: allocation,
                                                  connectionType: "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e.Message);
            return false;
        }

    }

    public async Task<bool> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new(allocation: joinAllocation,
                                                  connectionType: "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            return true;
        }
        catch (RelayServiceException e)
        {

            Debug.Log(e.Message);
            return false;
        }

    }
}
