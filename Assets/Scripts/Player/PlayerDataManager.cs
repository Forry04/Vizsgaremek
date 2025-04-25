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
            // XOR-based checksum validation
            double calculatedChecksum = CalculateXorChecksum(Coins, _coinsCheckKey);
            return Math.Abs(_coinsChecksum - calculatedChecksum) < 0.0001;
        }
    }

    private double CalculateXorChecksum(int coins, double key)
    {
        // XOR-based checksum calculation
        long coinsHash = coins ^ (int)(key * 1000000); // XOR coins with a scaled version of the key
        double checksum = coinsHash * 0.6180339887;    // Multiply by an irrational number for obfuscation
        return checksum;
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
                _coinsChecksum = CalculateXorChecksum(value, _coinsCheckKey); 
            }
        }
    }

    private int _coins;
    private double _coinsChecksum;
    private double _coinsCheckKey;
    

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
        _coinsCheckKey = GenerateSecureRandomKey();

        ScheduleNextCoinsCheck();
    }

    private double GenerateSecureRandomKey()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[8];
            rng.GetBytes(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
    }

    private void ScheduleNextCoinsCheck()
    {
        float randomInterval = UnityEngine.Random.Range(10f, 40f); // Random interval between 10 and 60 seconds
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