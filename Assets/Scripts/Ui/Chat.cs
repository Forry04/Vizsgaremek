using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(NetworkObject))]
public class Chat : NetworkBehaviour
{
    public static Chat Singleton;
    [SerializeField] private GameObject chatUiObject;
    [SerializeField] private string playerName = $"Player{NetworkManager.Singleton.LocalClient}";
    private VisualElement chatUi;

    private VisualElement chatContainer;
    private ScrollView chatListView;
    private TextField chatInputField;
    private bool isChatOpen = false;
    private bool ignoreNextReturn = false;

    private void Awake() => Singleton = this;

    private void Start()
    {

        chatUi = chatUiObject.GetComponent<UIDocument>().rootVisualElement;
        chatContainer = chatUi.Q<VisualElement>("chat-container");
        chatListView = chatUi.Q<ScrollView>("chat-window");
        chatInputField = chatUi.Q<TextField>("chat-input");

        chatInputField.RegisterCallback<KeyDownEvent>((evt) =>
        {
            if (evt.keyCode == KeyCode.Return && isChatOpen)
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

    // Listen to keyboard input to open chat
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatContainer.ClassListContains("hidden"))
            {
                chatContainer.RemoveFromClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true; // Enable the cursor
                StartCoroutine(FocusChatInputField()); // Focus the text field
                isChatOpen = true; // Set chat open flag
                ignoreNextReturn = true; // Ignore the next Return key press
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!chatContainer.ClassListContains("hidden"))
            {
                chatContainer.AddToClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false; // Disable the cursor
                isChatOpen = false; // Reset chat open flag
            }
        }
    }

    private void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        string s = $"{playerName}> {message}";
        ReceiveChatMessageRpc(s);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void ReceiveChatMessageRpc(string message)
    {
        chatListView.Add(new Label(message));
        if (chatContainer.ClassListContains("hidden"))
        {
            chatContainer.RemoveFromClassList("hidden");
            StartCoroutine(HideChatBox());
        }
        StartCoroutine(FocusChatInputField());

    }

    private IEnumerator HideChatBox()
    {
        yield return new WaitForSeconds(5);
        chatContainer.AddToClassList("hidden");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false; // Disable the cursor
        isChatOpen = false; // Reset chat open flag
    }

    private IEnumerator FocusChatInputField()
    {
        yield return null; // Wait for the next frame
        chatInputField.Focus();
    }
}
