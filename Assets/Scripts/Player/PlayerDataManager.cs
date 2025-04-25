using System;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Singleton { get; private set; }

    public string Name => $"Player {NetworkManager.Singleton.LocalClientId}";

    private readonly object _lock = new(); // Lock object for synchronization

    public bool IsCoinsValid()
    {
        lock (_lock)
        {
            // Simple checksum validation
            return _coinsChecksum == _coins;
        }
    }

    public int Coins
    {
        get
        {
            lock (_lock)
            {
                return _coins;
            }
        }
        set
        {
            lock (_lock)
            {
                if (value < 0)
                {
                    Debug.LogError("Coins value cannot be negative.");
                    return;
                }
                if (!IsCoinsValid())
                {
                    Debug.LogError("Coins checksum is invalid. Cannot set coins.");
                    _coins = 0;
                    _coinsChecksum = 0;
                    return;
                }

                _coins = value;
                _coinsChecksum = value; // Update checksum to match the coins value
            }
        }
    }

    private int _coins;
    private int _coinsChecksum;

    private void Awake()
    {
        // Ensure thread-safe singleton
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);

        ScheduleNextCoinsCheck();
    }

    private void ScheduleNextCoinsCheck()
    {
        float randomInterval = UnityEngine.Random.Range(10f, 40f); // Random interval between 10 and 40 seconds
        Invoke(nameof(PeriodicCoinsCheck), randomInterval);
    }

    private void PeriodicCoinsCheck()
    {
        lock (_lock)
        {
            if (!IsCoinsValid())
            {
                Debug.LogError("Periodic check failed: Coins checksum is invalid!");
            }
        }

        ScheduleNextCoinsCheck();
    }
}