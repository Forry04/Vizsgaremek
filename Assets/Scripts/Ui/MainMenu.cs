using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

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


    private async void OnHostClicked()
    {
        mainMenuUiObject.SetActive(false);
        if (await Relay.Singleton.CreateRelay())
        {
            Debug.Log("Hosted");
        }
        else
        {
            mainMenuUiObject.SetActive(true);
        }

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
