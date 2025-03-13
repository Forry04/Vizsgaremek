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
    [SerializeField] private string playerName = $"Player{NetworkManager.Singleton.LocalClient.ClientId}";
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
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    UnityEngine.Cursor.visible = false;
                }
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatContainer.ClassListContains("hidden"))
            {
                chatContainer.RemoveFromClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                StartCoroutine(FocusChatInputField());
                isChatOpen = true;
                ignoreNextReturn = true;
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
                UnityEngine.Cursor.visible = false;
                isChatOpen = false;
            }
        }
    }

    private void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        string s = $"{playerName} > {message}";
        ReceiveChatMessageRpc(s);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void ReceiveChatMessageRpc(string message)
    {
        chatListView.Add(new Label(message));
        chatContainer.RemoveFromClassList("hidden");
        chatListView.scrollOffset = new Vector2(0, chatListView.contentContainer.layout.height);
        StartCoroutine(HideChatBox());
    }

    private IEnumerator HideChatBox()
    {
        yield return new WaitForSeconds(5);
        chatContainer.AddToClassList("hidden");
        isChatOpen = false;
    }

    private IEnumerator FocusChatInputField()
    {
        yield return null; // Wait for the next frame
        chatInputField.Focus();
    }
}
