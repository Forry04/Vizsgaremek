using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Ui Input Action Map Name References")]
    [SerializeField] private string uiActionMapName = "UI";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string fire = "Fire";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string unlockCamera = "UnlockCamera";
    [SerializeField] private string openChat = "OpenChat"; 
    [SerializeField] private string escapeMenu = "EscapeMenu";

    [Header("Ui Name References")]
    [SerializeField] private string submit = "Submit";
    [SerializeField] private string cancel = "Cancel";

    //actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction fireAction;
    private InputAction jumpAction;
    public InputAction crouchAction;
    private InputAction sprintAction;
    private bool crouchTriggered;
    private InputAction unlockCameraAction;
    private InputAction openChatAction;
    private InputAction escapeMenuAction;

    //ui
    private InputAction submitAction;
    private InputAction cancelAction;

    //gameplay
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool FireTriggered { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTriggered { get => crouchTriggered; private set => crouchTriggered = value; }
    public bool SprintTriggered { get; private set; }
    public bool LookDevice { get; private set; }
    public bool UnlockCameraTriggered { get; private set; }
    public bool OpenChatTriggered { get; private set; }
    public bool EscapeMenuTriggered { get; private set; }

    //ui
    public bool SubmitTriggered { get; private set; }
    public bool CancelTriggered { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        //gameplay
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        fireAction = playerControls.FindActionMap(actionMapName).FindAction(fire);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        crouchAction = playerControls.FindActionMap(actionMapName).FindAction(crouch);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        unlockCameraAction = playerControls.FindActionMap(actionMapName).FindAction(unlockCamera);
        openChatAction = playerControls.FindActionMap(actionMapName).FindAction(openChat);
        escapeMenuAction = playerControls.FindActionMap(actionMapName).FindAction(escapeMenu);

        //ui
        submitAction = playerControls.FindActionMap(uiActionMapName).FindAction(submit);
        cancelAction = playerControls.FindActionMap(uiActionMapName).FindAction(cancel);
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        //gameplay
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        fireAction.performed += context => FireTriggered = true;
        fireAction.canceled += context => FireTriggered = false;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        crouchAction.started += context => crouchTriggered = true;

        sprintAction.performed += context => SprintTriggered = true;
        sprintAction.canceled += context => SprintTriggered = false;

        unlockCameraAction.performed += context => UnlockCameraTriggered = true;
        unlockCameraAction.canceled += context => UnlockCameraTriggered = false;

        openChatAction.performed += context => OpenChatTriggered = true;
        openChatAction.canceled += context => OpenChatTriggered = false;

        escapeMenuAction.performed += context => EscapeMenuTriggered = true;
        escapeMenuAction.canceled += context => EscapeMenuTriggered = false;

        //ui
        submitAction.performed += context => SubmitTriggered = true;
        submitAction.canceled += context => SubmitTriggered = false;

        cancelAction.performed += context => CancelTriggered = true;
        cancelAction.canceled += context => CancelTriggered = false;

    }



    private void OnEnable()
    {
        EnablePlayerActionMap();
        //crouchAction.started += OnCrouchStarted;
        //moveAction.Enable();
        //lookAction.Enable();
        //fireAction.Enable();
        //jumpAction.Enable();
        //crouchAction.Enable();
        //sprintAction.Enable();
    }
    private void OnDisable()
    {
        DisableAllActionMaps();
        //playerControls.FindActionMap(actionMapName).Disable();
        //lookAction.performed -= OnActionPerformed;
        //crouchAction.started -= OnCrouchStarted;
        //moveAction.Disable();
        //lookAction.Disable();
        //fireAction.Disable();
        //jumpAction.Disable();
        //crouchAction.Disable();
        //sprintAction.Disable();
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        // Get the device that triggered the action
        var device = context.control.device;

        switch (device)
        {
            case Mouse:
                LookDevice = true;
                break;
            case Gamepad:
                LookDevice = false;
                break;
            default:
                break;
        }
    }
    public void EnablePlayerActionMap()
    {
        playerControls.FindActionMap(actionMapName).Enable();
        playerControls.FindActionMap(uiActionMapName).Disable();
    }
    public void EnableUIActionMap()
    {
        playerControls.FindActionMap(actionMapName).Disable();
        playerControls.FindActionMap(uiActionMapName).Enable();
    }

    private void DisableAllActionMaps()
    {
        playerControls.FindActionMap(actionMapName).Disable();
        playerControls.FindActionMap(uiActionMapName).Disable();
    }

    private void FixedUpdate()
    {
        if (crouchTriggered) crouchTriggered = false;
    }
}
