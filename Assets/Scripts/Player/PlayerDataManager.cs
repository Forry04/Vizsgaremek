using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Singleton { get; private set; }
    public string Name { get => NetworkManager.Singleton.LocalClientId.ToString(); }

    private void Awake()
    {
        if (Singleton == null || Singleton != this)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
