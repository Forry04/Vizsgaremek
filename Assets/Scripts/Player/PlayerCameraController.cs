using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : NetworkBehaviour
{
    [InspectorLabel("Mouse Sensitivity")]
    [Range(1, 100)]
    [SerializeField] private float mouseSensitivity = 18f;
    [Range(1, 500)]
    [InspectorLabel("Joystick Sensitivity")]
    [SerializeField] private float joystickSensitivity = 250f;
    [SerializeField] private float distanceFromPlayer = 5f;
    [SerializeField] private Vector3 offset = new(0, 2, 0);
    private (float X, float Y) rotation = (0, 0);
    private Transform orientation;
    private PlayerInputHandler inputHandler;
    private bool isCameraUnlocked = false;

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
        rotation.X -= mouseY;
        rotation.X = Mathf.Clamp(rotation.X, -90f, 90f);

        rotation.Y += mouseX;

        if (inputHandler.UnlockCameraTriggered)
        {
            isCameraUnlocked = true;
        }
        else if (isCameraUnlocked)
        {
            isCameraUnlocked = false;
            // Ensure the camera smoothly transitions back to the locked state
            rotation.Y = orientation.eulerAngles.y;
        }

        if (isCameraUnlocked)
        {
            Vector3 direction = new(0, 0, -distanceFromPlayer);
            Quaternion rotationQuat = Quaternion.Euler(rotation.X, rotation.Y, 0);
            transform.position = orientation.position + offset + rotationQuat * direction;
            transform.LookAt(orientation.position + offset);
        }
        else
        {
            orientation.localRotation = Quaternion.Euler(0f, rotation.Y, 0f);
        }
    }
}
