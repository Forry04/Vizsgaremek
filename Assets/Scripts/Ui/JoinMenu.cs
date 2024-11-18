using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUiObject;
    [SerializeField] private GameObject joinMenuUiObject;
    [SerializeField] private GameObject loadingMenUiObject;

    private VisualElement joinMenuUi;
    private VisualElement mainContainer;
    private Button backButton;
    private Button joinButton;
    private TextField joinCodeField;
    private void OnEnable()
    {
        joinMenuUi = joinMenuUiObject.GetComponent<UIDocument>().rootVisualElement;

        mainContainer = joinMenuUi.Q<VisualElement>("main-container");
        joinButton = joinMenuUi.Q<Button>("join-button");
        backButton = joinMenuUi.Q<Button>("back-button");
        joinCodeField = joinMenuUi.Q<TextField>("join-code-field");

        backButton.clicked += OnBackClicked;
        joinButton.clicked += OnJoinClicked;

    }
    
    private void OnBackClicked()
    {
        mainMenuUiObject.SetActive(true);
        joinMenuUiObject.SetActive(false);
    }

    private async void OnJoinClicked()
    {
       
        mainContainer.visible = false;

        if (joinCodeField.text is null || joinCodeField.text.Equals(string.Empty))
        {
            Debug.LogError("Joicode invalid");
            mainContainer.visible = true;
            return;
        }

        loadingMenUiObject.SetActive(true);
        if (await Relay.Singleton.JoinRelay(joinCodeField.text))
        {
            Debug.Log("Joined");
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", default);
        }
        else
        { 
            Debug.LogError("Failed to join");
            mainContainer.visible = true;
            loadingMenUiObject.SetActive(false);
        }
    }


}
