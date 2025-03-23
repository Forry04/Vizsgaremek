using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
  
    private PlayerInputHandler playerInput;
    [SerializeField]
    private GameObject pauseMenuUiObject;

    private VisualElement pauseMenuUi;
    private Button resumeButton;
    private Button settingButton;
    private Button readyButton;
    private Button startButton;
    private Button exitButton;

    private bool isReady = false;

  

    private void OnEnable()
    {
        
        playerInput = Chat.Singleton.playerInput;


        // Load the UXML file and get the root visual element
        pauseMenuUi = pauseMenuUiObject.GetComponent<UIDocument>().rootVisualElement;

        // Query the buttons by their names
        resumeButton = pauseMenuUi.Q<Button>("resume-button");
        settingButton = pauseMenuUi.Q<Button>("setting-button");
        readyButton = pauseMenuUi.Q<Button>("ready-button");
        startButton = pauseMenuUi.Q<Button>("start-button");
        exitButton = pauseMenuUi.Q<Button>("exit-button");

        // Add event listeners to the buttons
        resumeButton.clicked += OnResumeClicked;
        settingButton.clicked += OnSettingClicked;
        readyButton.clicked += OnReadyClicked;
        startButton.clicked += OnStartClicked;
        exitButton.clicked += OnExitClicked;

        pauseMenuUi.RegisterCallback<KeyDownEvent>((evt) =>
        {
            if (playerInput.CancelTriggered)
            {
                pauseMenuUiObject.SetActive(false);
            }
        });

    }

    private void OnDisable()
    {
        playerInput.EnablePlayerActionMap();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
   
        resumeButton.clicked -= OnResumeClicked;
        settingButton.clicked -= OnSettingClicked;
        readyButton.clicked -= OnReadyClicked;
        startButton.clicked -= OnStartClicked;
        exitButton.clicked -= OnExitClicked;
    }

    private void OnResumeClicked()
    {
        playerInput.EnablePlayerActionMap();
        pauseMenuUiObject.SetActive(false);
    }

    private void OnSettingClicked()
    {

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
       
    }

}
