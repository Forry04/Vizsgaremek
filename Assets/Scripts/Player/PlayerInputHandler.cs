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

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string fire = "Fire";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string unlockCamera = "UnlockCamera";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction fireAction;
    private InputAction jumpAction;
    public InputAction crouchAction;
    private InputAction sprintAction;
    private bool crouchTriggered;
    private InputAction unlockCameraAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool FireTriggered { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTriggered { get => crouchTriggered; private set => crouchTriggered = value; }
    public bool SprintTriggered { get; private set; }
    public bool LookDevice { get; private set; }
    public bool UnlockCameraTriggered { get; private set; }
    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        fireAction = playerControls.FindActionMap(actionMapName).FindAction(fire);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        crouchAction = playerControls.FindActionMap(actionMapName).FindAction(crouch);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        unlockCameraAction = playerControls.FindActionMap(actionMapName).FindAction(unlockCamera);
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
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
    }



    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
        lookAction.performed += OnActionPerformed;
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
        playerControls.FindActionMap(actionMapName).Disable();
        lookAction.performed -= OnActionPerformed;
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

    private void FixedUpdate()
    {
        if (crouchTriggered) crouchTriggered = false;
    }
}
