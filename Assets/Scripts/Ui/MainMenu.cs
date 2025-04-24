using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUiObject;
    [SerializeField] private GameObject joinMenuUiObject;
    [SerializeField] private GameObject loadingMenUiObject;
    [SerializeField] private GameObject SettingsMenUiObject;
    [SerializeField] private GameObject LogInMenuUIObject;
    [SerializeField] private GameObject customMenuUIObject;
    public AudioManager audioManager => AudioManager.Instance;

    private VisualElement mainMenuUi;
    private VisualElement mainContainer;
    private VisualElement joinMenuUi;

    private Button hosttButton;
    private Button joinButton;
    private Button setingstButton;
    private Button exitButton;
    private Button logOutButton;
    private Button customButton;

    private ConfirmationPopup confirmationPopup;

    private void OnEnable()
    {

        mainMenuUi = mainMenuUiObject.GetComponent<UIDocument>().rootVisualElement;
        

        mainContainer = mainMenuUi.Q<VisualElement>("main-container");
        hosttButton = mainMenuUi.Q<Button>("host-button");
        joinButton = mainMenuUi.Q<Button>("join-button");
        setingstButton = mainMenuUi.Q<Button>("settings-button");
        exitButton = mainMenuUi.Q<Button>("exit-button");
        logOutButton = mainMenuUi.Q<Button>("LogOut-button");
        customButton = mainMenuUi.Q<Button>("Costum-button");

        hosttButton.clicked += OnHostClicked;
        joinButton.clicked += OnJoinClicked;
        setingstButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;
        logOutButton.clicked += OnLogOutClicked;
        customButton.clicked += OnCustomClicked;

        confirmationPopup = gameObject.AddComponent<ConfirmationPopup>();

        
        confirmationPopup.Initialize(mainMenuUi);



        StartCoroutine(SetupAudioCallbacks());
       
    }

    private void OnCustomClicked()
    {
        gameObject.SetActive(false);
        customMenuUIObject.SetActive(true);
    }

    private IEnumerator SetupAudioCallbacks()
    {
        while (audioManager == null) yield return null;
        var buttons = GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList();
            foreach (var button in buttons)
            {
                button.RegisterCallback<MouseEnterEvent>(evt =>
                {

                    audioManager.Play("ButtonHover");
                });

                button.RegisterCallback<FocusEvent>(evt =>
                {

                    audioManager.Play("ButtonHover");
                });

                button.RegisterCallback<ClickEvent>(evt =>
                {
                    //PlayClickSound();
                });
            }
        hosttButton.Focus();
    }

    private async void OnHostClicked()
    {

        
        mainContainer.visible = false;
        loadingMenUiObject.SetActive(true);
        if (await Relay.Singleton.CreateRelay())
        {
            Debug.Log("Hosted");
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby",default);
        }
        else
        {
            Debug.LogError("Failed to host");
            loadingMenUiObject.SetActive(false);
            mainContainer.visible = true;

        }

    }

    private void OnLogOutClicked()
    {
        confirmationPopup.Show(
            "Are you sure you want to log out?",
            onConfirm: () =>
            {
                PlayerPrefs.DeleteKey("token");
                gameObject.SetActive(false);
                LogInMenuUIObject.SetActive(true);

            },
            onCancel: () =>
            {
                // Do nothing on cancel
            });
    }

    private void OnJoinClicked()
    {
        gameObject.SetActive(false);
        joinMenuUiObject.SetActive(true);

    }

    private void OnSettingsClicked()
    {
        gameObject.SetActive(false);
        SettingsMenUiObject.SetActive(true);   
    }

    private void OnExitClicked()
    {
        

        confirmationPopup.Show(
          message: "Are you sure you want to exit?",
         onConfirm: () =>
        {
            Application.Quit();
            Debug.Log("Exit");
        },
         onCancel: () =>
        {
           
        });
        
    }
   

}
