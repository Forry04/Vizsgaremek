using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(NetworkObject))]
public class Chat : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;

    public static Chat Singleton;
    [SerializeField] private GameObject chatUiObject;
    [SerializeField] private string playerName = $"Player{NetworkManager.Singleton.LocalClient.ClientId}";
    private VisualElement chatUi;

    private VisualElement chatContainer;
    private VisualElement tempContainer;
    private ScrollView chatScrollView;
    private TextField chatInputField;
    private bool isChatOpen = false;
    private bool ignoreNextReturn = false;

    /// <summary>
    /// Initializes the singleton instance and player name.
    /// </summary>
    private void Awake()
    {
        Singleton = this;
       
    }

    /// <summary>
    /// Initializes the chat UI elements and registers the key down event for the chat input field.
    /// </summary>
    private void Start()
    {
        

        DontDestroyOnLoad(this.gameObject);
        chatUi = chatUiObject.GetComponent<UIDocument>().rootVisualElement;
        chatContainer = chatUi.Q<VisualElement>("chat-container");
        tempContainer = chatUi.Q<VisualElement>("temp-container");
        chatScrollView = chatUi.Q<ScrollView>("chat-window");
        chatInputField = chatUi.Q<TextField>("chat-input");

        chatInputField.RegisterCallback<KeyDownEvent>((evt) =>
        {
            if (inputHandler.SubmitTriggered && isChatOpen)
            {
                if (ignoreNextReturn)
                {
                    ignoreNextReturn = false;
                    return;
                }

                if (!string.IsNullOrEmpty(chatInputField.value))
                {
                    string message = chatInputField.value;
                    chatInputField.value = string.Empty;
                    SendChatMessage(message);
                }
            }
        });
    }

    /// <summary>
    /// Handles the Return and Escape key inputs to open/close the chat and send messages.
    /// </summary>
    private void Update()
    {
        if (inputHandler.OpenChatTriggered)
        {
            if (chatContainer.ClassListContains("hidden"))
            {
                tempContainer.AddToClassList("hidden");
                chatContainer.RemoveFromClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                StartCoroutine(FocusChatInputField());
                isChatOpen = true;
                ignoreNextReturn = true;
                inputHandler.EnableUIActionMap();
            }
            else if (isChatOpen)
            {
                if (!string.IsNullOrEmpty(chatInputField.value))
                {
                    string message = chatInputField.value;
                    chatInputField.value = string.Empty;
                    SendChatMessage(message);
                }
            }
        }

        if (inputHandler.CancelTriggered)
        {
            if (!chatContainer.ClassListContains("hidden"))
            {
                chatContainer.AddToClassList("hidden");
                tempContainer.RemoveFromClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                isChatOpen = false;
                inputHandler.EnablePlayerActionMap();
            }
        }
    }

    /// <summary>
    /// Sends a chat message to all clients and closes the chat box.
    /// </summary>
    /// <param name="message">The message to send.</param>
    private void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        string s = $"{playerName} > {message}";
        chatContainer.AddToClassList("hidden");
        tempContainer.RemoveFromClassList("hidden");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        inputHandler.EnablePlayerActionMap();
        isChatOpen = false;
        ReceiveChatMessageRpc(s);
    }

    /// <summary>
    /// Receives a chat message and displays it in the chat UI.
    /// </summary>
    /// <param name="message">The message to display.</param>
    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void ReceiveChatMessageRpc(string message)
    {
        chatScrollView.Add(new Label(message));
        if (!chatContainer.ClassListContains("hidden"))
        {
            chatScrollView.scrollOffset = new Vector2(0, chatScrollView.contentContainer.layout.height);
        }
        else
        {
            Label tempLabel = new(message);
            tempLabel.AddToClassList("temp");
            tempContainer.Add(tempLabel);
            StartCoroutine(RemoveTempMessage(tempLabel));
        }
    }

    /// <summary>
    /// Removes a temporary message from the chat UI after a delay.
    /// </summary>
    /// <param name="lbl">The label to remove.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator RemoveTempMessage(Label lbl)
    {
        yield return new WaitForSeconds(5);
        tempContainer.Remove(lbl);
    }

    /// <summary>
    /// Focuses the chat input field.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator FocusChatInputField()
    {
        yield return null;
        chatInputField.Focus();
    }
}
