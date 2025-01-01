using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void Update()
    {

    }
    private void OnEnable()
    {
        joinMenuUi = joinMenuUiObject.GetComponent<UIDocument>().rootVisualElement;

        mainContainer = joinMenuUi.Q<VisualElement>("main-container");
        joinButton = joinMenuUi.Q<Button>("join-button");
        backButton = joinMenuUi.Q<Button>("back-button");
        joinCodeField = joinMenuUi.Q<TextField>("join-code-field");

        backButton.clicked += OnBackClicked;
        joinButton.clicked += OnJoinClicked;

        joinCodeField.textEdition.maxLength = 6;

        joinCodeField.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            joinCodeField.value = evt.newValue.ToUpper();
        });

        joinCodeField.RegisterCallback<KeyDownEvent>((evt) =>
        {
            if (evt.keyCode == KeyCode.Return)
            {
                OnJoinClicked();
            }
            else if (evt.keyCode == KeyCode.Escape)
            {
                OnBackClicked();
            }

        });

        joinCodeField.Focus();
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
            Debug.LogError("Join code invalid");
            mainContainer.visible = true;
            return;
        }

        loadingMenUiObject.SetActive(true);
        if (await Relay.Singleton.JoinRelay(joinCodeField.text))
        {
            Debug.Log("Joined");
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            Debug.LogError("Failed to join");
            mainContainer.visible = true;
            loadingMenUiObject.SetActive(false);
        }
    }

  
}
