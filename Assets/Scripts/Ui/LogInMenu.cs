using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LogInMenu : MonoBehaviour
{
    private VisualElement logInMenuUI;

    private Button loginButton;
    private Button accountButton;
    private Button resetPassButton;

    private TextField userNameTextField;
    private TextField passwordTextField;

    private Label errorLabel;

    private string errorMassage = "*Invalid username or password";
    public AudioManager AudioManager => AudioManager.Instance;

    [SerializeField] private GameObject MainMenuUiObject;

    List<Button> Buttons;
    private void OnEnable()
    {
        logInMenuUI = gameObject.GetComponent<UIDocument>().rootVisualElement;

        loginButton = logInMenuUI.Q<Button>("LogIn-button");
        accountButton = logInMenuUI.Q<Button>("Account-button");
        resetPassButton = logInMenuUI.Q<Button>("ResetPassword-button");

        userNameTextField = logInMenuUI.Q<TextField>("UserName-textfield");
        passwordTextField = logInMenuUI.Q<TextField>("Password-textfield");

        errorLabel = logInMenuUI.Q<Label>("Error-label");
        errorLabel.text = errorMassage;
        errorLabel.visible = false;

        Buttons = GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList();

  

        loginButton.clicked += OnLoginClicked;
        accountButton.clicked += OnAccountClicked;
        resetPassButton.clicked += OnResetPassClicked;
        AssignButtonSounds();

        userNameTextField.Focus();

    }

    private void OnLoginClicked()
    {
        if (userNameTextField.text == "admin" && passwordTextField.text == "admin")
        {
            PlayerPrefs.SetString("token","jah");
            MainMenuUiObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else errorLabel.visible = true;      
    }

    private void OnAccountClicked()
    {
        Application.OpenURL("https://www.youtube.com/");
    }

    private void OnResetPassClicked()
    {
        Application.OpenURL("https://www.youtube.com/");
    }

    private void AssignButtonSounds()
    {
        foreach (var button in Buttons)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {

                AudioManager.Play("ButtonHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {

                AudioManager.Play("ButtonHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }
    }


}
