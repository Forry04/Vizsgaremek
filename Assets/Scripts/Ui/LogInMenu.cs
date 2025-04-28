using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
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
    [SerializeField] private string loginUrl = "http://localhost:3000/api/auth/login"; // Replace with your API endpoint

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

    private async void OnLoginClicked()
    {
        string email = userNameTextField.text.Trim().ToLower();
        string password = passwordTextField.text.Trim();

        await LoginAsync(email, password);
    }

    private async Task LoginAsync(string email, string password)
    {
        var loginData = new
        {
            email = email,
            password = password
        };

        using (HttpClient client = new HttpClient())
        {
            try
            {
                string json = JsonConvert.SerializeObject(loginData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(loginUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Login successful: " + responseBody);

                    // Parse the response (e.g., extract the token)
                    var responseData = JsonConvert.DeserializeObject<LoginResponse>(responseBody);
                    PlayerPrefs.SetString("token", responseData.token);

                    // Navigate to the main menu
                    MainMenuUiObject.SetActive(true);
                    gameObject.SetActive(false);
                     await  GetComponentInParent<ManageUI>().GetUserName(responseData.token);
                }
                else
                {
                    Debug.LogError("Login failed: " + response.StatusCode);
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error details: " + errorResponse);

                    // Display error message
                    errorLabel.text = "*Invalid username or password";
                    errorLabel.visible = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occurred: " + ex.Message);
                errorLabel.text = "*Error logging in";
                errorLabel.visible = true;
            }
        }
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

    [Serializable]
    private class LoginResponse
    {
        public string token;
    }
}

// Define a class to deserialize the API response
[Serializable]
internal class LoginResponse
{
    public string token;
}
