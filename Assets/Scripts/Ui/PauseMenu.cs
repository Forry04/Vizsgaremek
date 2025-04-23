using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using System;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject SettingsMenuUiObject;
    [SerializeField] private GameObject CustomMenuUiObject;

    private PlayerInputHandler playerInput;
    private VisualElement pauseMenuUi;
    private Button resumeButton;
    private Button settingButton;
    private Button readyButton;
    private Button startButton;
    private Button exitButton;
    private Button customButton;
    private Button backButton;

    private bool isReady = false;

    private ConfirmationPopup confirmationPopup;

    private void OnEnable()
    {
        playerInput = Chat.Singleton.playerInput;

        // Load the UXML file and get the root visual element
        pauseMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;

        // Query the buttons by their names
        resumeButton = pauseMenuUi.Q<Button>("resume-button");
        settingButton = pauseMenuUi.Q<Button>("setting-button");
        readyButton = pauseMenuUi.Q<Button>("ready-button");
        startButton = pauseMenuUi.Q<Button>("start-button");
        exitButton = pauseMenuUi.Q<Button>("exit-button");
        customButton = pauseMenuUi.Q<Button>("custom-button");
        backButton = pauseMenuUi.Q<Button>("back-button");

        // Add event listeners to the buttons
        resumeButton.clicked += OnResumeClicked;
        settingButton.clicked += OnSettingClicked;
        readyButton.clicked += OnReadyClicked;
        startButton.clicked += OnStartClicked;
        exitButton.clicked += OnExitClicked;
        customButton.clicked += OnCostumClicked;
        backButton.clicked += OnBackClicked;

        //pauseMenuUi.RegisterCallback<KeyDownEvent>((evt) =>
        //{
        //    if (playerInput.CancelTriggered)
        //    {
        //        gameObject.SetActive(false);
        //    }
        //});

        var buttons = GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList();
        foreach (var button in buttons)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {
                FindObjectOfType<AudioManager>().Play("ButtonHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {
                FindObjectOfType<AudioManager>().Play("ButtonHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }

        if (NetworkManager.Singleton.IsHost)
        {
            startButton.style.display = DisplayStyle.Flex;
        }

        confirmationPopup = gameObject.AddComponent<ConfirmationPopup>();
        confirmationPopup.Initialize(pauseMenuUi.Q<VisualElement>("main"));

       

        pauseMenuUi.Focus();
    }

    private void OnDisable()
    {
        resumeButton.clicked -= OnResumeClicked;
        settingButton.clicked -= OnSettingClicked;
        readyButton.clicked -= OnReadyClicked;
        startButton.clicked -= OnStartClicked;
        exitButton.clicked -= OnExitClicked;
        customButton.clicked -= OnCostumClicked;
        backButton.clicked -= OnBackClicked;
    }

    private void OnResumeClicked()
    {
        playerInput.EnablePlayerActionMap();
        gameObject.SetActive(false);

        playerInput.EnablePlayerActionMap();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnSettingClicked()
    {
        gameObject.SetActive(false);
        SettingsMenuUiObject.SetActive(true);
    }

    private void OnReadyClicked()
    {
        isReady = !isReady;
        readyButton.text = isReady ? "Unready" : "Ready";
        ReadyStartSystem.Singleton.SetPlayerState(NetworkManager.Singleton.LocalClientId);
    }

    private void OnStartClicked()
    {
    }

    private void OnExitClicked()
    {
        confirmationPopup.Show("Are you sure you want to exit?", () =>
        {
            Application.Quit();
        }, () =>
        {
            // Do nothing on cancel
        });
    }

    private void OnBackClicked()
    {
        confirmationPopup.Show(
            "Are you sure you want to return to the main menu?",
            onConfirm: () =>
            {
              
                if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.Shutdown();
                }

                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
            },
            onCancel: () =>
            {
                // Do nothing on cancel
            });
    }

    private void OnCostumClicked()
    {
        gameObject.SetActive(false);
        CustomMenuUiObject.SetActive(true);
    }
}