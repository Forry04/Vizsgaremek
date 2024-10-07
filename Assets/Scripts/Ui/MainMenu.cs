using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUiObject;
    [SerializeField] private GameObject joinMenuUiObject;

    private VisualElement mainMenuUi;
    private Button hosttButton;
    private Button joinButton;
    private Button setingstButton;
    private Button exitButton;


    private void OnEnable()
    {

        mainMenuUi = mainMenuUiObject.GetComponent<UIDocument>().rootVisualElement;

        hosttButton = mainMenuUi.Q<Button>("host-button");
        joinButton = mainMenuUi.Q<Button>("join-button");
        setingstButton = mainMenuUi.Q<Button>("settings-button");
        exitButton = mainMenuUi.Q<Button>("exit-button");

        hosttButton.clicked += OnHostClicked;
        joinButton.clicked += OnJoinClicked;
        setingstButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;


    }


    private void OnHostClicked()
    {
        Relay.Singleton.CreateRelay();
    }

    private void OnJoinClicked()
    {
        mainMenuUiObject.SetActive(false);
        joinMenuUiObject.SetActive(true);

    }

    private void OnSettingsClicked()
    {
        throw new NotImplementedException();
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

}
