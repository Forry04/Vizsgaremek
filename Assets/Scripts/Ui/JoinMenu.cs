using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUiObject;
    [SerializeField] private GameObject joinMenuUiObject;

    private VisualElement joinMenuUi;
    private Button backButton;
    private Button joinButton;
    private TextField joinCodeField;
    private void OnEnable()
    {
        joinMenuUi = joinMenuUiObject.GetComponent<UIDocument>().rootVisualElement;


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

    private void OnJoinClicked()
    {
        Relay.Singleton.JoinRelay(joinCodeField.text);
    }

}
