using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(NetworkObject))]
public class Chat : NetworkBehaviour
{
    public static Chat Singleton;

    public PlayerInputHandler playerInput;

    [SerializeField] private GameObject chatUiObject;
    [SerializeField] private string playerName => PlayerDataManager.Singleton.Name;
    private VisualElement chatUi;

    private VisualElement chatContainer;
    private VisualElement tempContainer;
    private ScrollView chatScrollView;
    private TextField chatInputField;
    private bool isChatOpen = false;
    private bool ignoreNextReturn = false;

    private void Awake() => Singleton = this;

    private void Start()
    {
        chatUi = chatUiObject.GetComponent<UIDocument>().rootVisualElement;
        chatContainer = chatUi.Q<VisualElement>("chat-container");
        tempContainer = chatUi.Q<VisualElement>("temp-container");
        chatScrollView = chatUi.Q<ScrollView>("chat-window");
        chatInputField = chatUi.Q<TextField>("chat-input");

        chatInputField.RegisterCallback<KeyDownEvent>((evt) =>
        {
            if (playerInput.SubmitTriggered && isChatOpen)
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
                    SendChatMessage(message, Color.white);
                }
            }
        });
    }

    private void Update()
    {
        if (playerInput.OpenChatTriggered)
        {
            playerInput.EnableUIActionMap();
            if (chatContainer.ClassListContains("hidden"))
            {
                tempContainer.AddToClassList("hidden");
                chatContainer.RemoveFromClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                StartCoroutine(FocusChatInputField());
                isChatOpen = true;
                ignoreNextReturn = true;
            }
        }

        if (playerInput.CancelTriggered)
        {
            if (!chatContainer.ClassListContains("hidden"))
            {
                chatContainer.AddToClassList("hidden");
                tempContainer.RemoveFromClassList("hidden");
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                isChatOpen = false;
                playerInput.EnablePlayerActionMap();
            }
        }
    }

    public void SendAnnouncement(string message, Color color)
    {
        string s = $"System > {message}";
        ReceiveChatMessageRpc(s, color);
    }

    private void SendChatMessage(string message, Color color)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        string s = $"{playerName} > {message}";
        chatContainer.AddToClassList("hidden");
        tempContainer.RemoveFromClassList("hidden");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        isChatOpen = false;
        playerInput.EnablePlayerActionMap();
        ReceiveChatMessageRpc(s, color);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void ReceiveChatMessageRpc(string message, Color color)
    {
        Label messageLabel = CreateColoredLabel(message, color);
        chatScrollView.Add(messageLabel);
        chatScrollView.ScrollTo(messageLabel);
        if (!chatContainer.ClassListContains("hidden"))
        {
            chatScrollView.scrollOffset = new Vector2(0, chatScrollView.contentContainer.layout.height);
        }
        else
        {
            Label tempLabel = CreateColoredLabel(message, color);
            tempLabel.AddToClassList("temp");
            tempContainer.Add(tempLabel);
            StartCoroutine(RemoveTempMessage(tempLabel));
        }
    }

    private Label CreateColoredLabel(string text, Color color)
    {
        Label label = new(text);
        label.style.color = new StyleColor(color);
        return label;
    }

    private IEnumerator RemoveTempMessage(Label lbl)
    {
        yield return new WaitForSeconds(5);
        tempContainer.Remove(lbl);
    }

    private IEnumerator FocusChatInputField()
    {
        yield return null;
        chatInputField.Focus();
    }
}
