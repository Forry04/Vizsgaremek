using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        NetworkManager.Singleton.StartHost();
    }
}
