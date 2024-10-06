using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuUiDoc;
    [SerializeField] private UIDocument joinMenuUiDoc;

    private VisualElement mainMenuUi;
    private VisualElement joinMenuUi;
    private Button hosttButton;
    private Button joinButton;
    private Button setingstButton;
    private Button exitButton;

   
    private void Awake()
    {
       
        mainMenuUi = mainMenuUiDoc.rootVisualElement;

        hosttButton = mainMenuUi.Q<Button>("host-button");
        joinButton = mainMenuUi.Q<Button>("join-button");
        setingstButton = mainMenuUi.Q<Button>("settings-button");
        exitButton = mainMenuUi.Q<Button>("exit-button");

        hosttButton.clicked += OnHostClicked;
        joinButton.clicked += OnJoinClicked;
        setingstButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;

        joinMenuUi = joinMenuUiDoc.rootVisualElement;
        

    }

   
    private void OnHostClicked()
    {
        Relay.Singleton.CreateRelay();
    }

    private void OnJoinClicked()
    {
        mainMenuUiDoc.enabled = false;
        joinMenuUiDoc.enabled = true;
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
