using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages player data, including coins, and provides anti-tampering mechanisms.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the PlayerDataManager.
    /// </summary>
    public static PlayerDataManager Singleton { get; private set; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name  => $"Player {NetworkManager.Singleton.LocalClientId}";

    /// <summary>
    /// The player's coin count. The value is obfuscated and validated to prevent tampering.
    /// </summary>
    public int Coins
    {
        get => Decrypt(obfuscatedCoins);
        set
        {
            obfuscatedCoins = Encrypt(value);
            coinsHash = ComputeHash(value);
        }
    }

    private int obfuscatedCoins;
    private string coinsHash;
    private int obfuscationKey;
    private const float ValidationInterval = 30f;

    private void Awake()
    {
        // Ensure that only one instance of PlayerDataManager exists
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        DontDestroyOnLoad(gameObject);

        // Initialize the obfuscation key and coins
        obfuscationKey = GenerateSecureKey();
    }

    private void Start()
    {
        StartCoroutine(PeriodicValidation());
    }

    #region Anti-Coin Tampering Methods

    /// <summary>
    /// Encrypts the given value using the obfuscation key.
    /// </summary>
    /// <param name="value">The value to encrypt.</param>
    /// <returns>The encrypted value.</returns>
    private int Encrypt(int value) => value ^ obfuscationKey;

    /// <summary>
    /// Decrypts the given value using the obfuscation key.
    /// </summary>
    /// <param name="value">The value to decrypt.</param>
    /// <returns>The decrypted value.</returns>
    private int Decrypt(int value) => value ^ obfuscationKey;

    /// <summary>
    /// Computes a hash for the given value using SHA-256.
    /// </summary>
    /// <param name="value">The value to hash.</param>
    /// <returns>The computed hash as a Base64 string.</returns>
    private string ComputeHash(int value)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()));
            return Convert.ToBase64String(bytes);
        }
    }

    /// <summary>
    /// Validates the integrity of the coins value by comparing its hash.
    /// </summary>
    /// <returns>True if the coins value is valid; otherwise, false.</returns>
    public bool IsCoinsValid() => coinsHash == ComputeHash(Decrypt(obfuscatedCoins));

    /// <summary>
    /// Periodically validates the coins value to detect tampering.
    /// </summary>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator PeriodicValidation()
    {
        while (true)
        {
            yield return new WaitForSeconds(ValidationInterval);

            if (!IsCoinsValid())
            {
                Debug.LogWarning("Tampering detected! Resetting coins value.");
                Coins = 0;
                TriggerAntiCheat();
            }
        }
    }

    /// <summary>
    /// Triggers anti-cheat actions when tampering is detected.
    /// </summary>
    private void TriggerAntiCheat()
    {
        Debug.LogError("Anti-cheat triggered! Possible tampering detected.");
        throw new Exception("Anti-cheat triggered! Possible tampering detected.");
    }

    /// <summary>
    /// Generates a cryptographically secure key for obfuscation.
    /// </summary>
    /// <returns>A secure random key.</returns>
    private int GenerateSecureKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            return BitConverter.ToInt32(randomBytes, 0) & int.MaxValue;
        }
    }

    #endregion
}
