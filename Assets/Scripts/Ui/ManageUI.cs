using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ManageUI : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUiObject;
    [SerializeField] private GameObject LoginMenuUiObject;
    private void Start()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("token")))
            MainMenuUiObject.SetActive(true);
        else
            LoginMenuUiObject.SetActive(true);

    }

}
