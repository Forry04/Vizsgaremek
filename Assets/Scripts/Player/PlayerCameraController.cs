using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : NetworkBehaviour
{
    [InspectorLabel("Mouse Sensitivity")]
    [Range(1,100)]
    [SerializeField] private float mouseSensitivity = 18f;
    [Range(1,500)]
    [InspectorLabel("Joystick Sensitivity")]
    [SerializeField] private float joystickSensitivity = 250f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Transform orientation;
    private PlayerInputHandler inputHandler;
    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        orientation = transform.parent;
        if (!IsLocalPlayer)
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
            return;
        }
    }

    private void Start()
    {
        inputHandler = transform.parent.GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleCamMovement();
    }

    private void HandleCamMovement()
    {
        float mouseX = inputHandler.LookInput.x * (inputHandler.LookDevice ? mouseSensitivity : joystickSensitivity) * Time.deltaTime;
        float mouseY = inputHandler.LookInput.y * (inputHandler.LookDevice ? mouseSensitivity : joystickSensitivity) * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        orientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
