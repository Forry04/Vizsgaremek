using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(NetworkObject))]
public class Chat : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;

    public static Chat Singleton;

    public PlayerInputHandler playerInput;

    [SerializeField] private GameObject chatUiObject;

    [SerializeField] private string playerName => $"Player{NetworkManager.Singleton.LocalClient.ClientId}";

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
        PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;

        DontDestroyOnLoad(this.gameObject);
        chatUi = chatUiObject.GetComponent<UIDocument>().rootVisualElement;
        chatContainer = chatUi.Q<VisualElement>("chat-container");
        tempContainer = chatUi.Q<VisualElement>("temp-container");
        chatScrollView = chatUi.Q<ScrollView>("chat-window");
        chatInputField = chatUi.Q<TextField>("chat-input");

        inputHandler = FindObjectOfType<PlayerInputHandler>();

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
                    SendChatMessage(message);
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
                inputHandler.EnableUIActionMap();
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
        playerInput.EnablePlayerActionMap();
        ReceiveChatMessageRpc(s);
    }

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