using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUiObject;
    [SerializeField] private GameObject joinMenuUiObject;
    [SerializeField] private GameObject loadingMenUiObject;

    private VisualElement mainMenuUi;
    private VisualElement mainContainer;
    private VisualElement joinMenuUi;
    private Button hosttButton;
    private Button joinButton;
    private Button setingstButton;
    private Button exitButton;


    private void OnEnable()
    {

        mainMenuUi = mainMenuUiObject.GetComponent<UIDocument>().rootVisualElement;
        

        mainContainer = mainMenuUi.Q<VisualElement>("main-container");
        hosttButton = mainMenuUi.Q<Button>("host-button");
        joinButton = mainMenuUi.Q<Button>("join-button");
        setingstButton = mainMenuUi.Q<Button>("settings-button");
        exitButton = mainMenuUi.Q<Button>("exit-button");

        hosttButton.clicked += OnHostClicked;
        joinButton.clicked += OnJoinClicked;
        setingstButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;

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

    private void OnJoinClicked()
    {
        gameObject.SetActive(false);
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
