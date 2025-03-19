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
    [SerializeField] private float rotationSmoothTime = 0.1f;
    private (float X, float Y) rotation = (0, 0);
    private Transform orientation;
    private PlayerInputHandler inputHandler;
    private Vector2 currentRotation;
    private Vector2 rotationVelocity;

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
        float inputX = inputHandler.LookInput.x * (!inputHandler.LookDevice ? mouseSensitivity : joystickSensitivity) * Time.deltaTime;
        float inputY = inputHandler.LookInput.y * (!inputHandler.LookDevice ? mouseSensitivity : joystickSensitivity) * Time.deltaTime;
        rotation.X -= inputY;
        rotation.X = Mathf.Clamp(rotation.X, -90f, 90f);

        rotation.Y += inputX;

        // Smoothly interpolate the camera rotation
        currentRotation = Vector2.SmoothDamp(currentRotation, new Vector2(rotation.X, rotation.Y), ref rotationVelocity, rotationSmoothTime);

        // Apply the rotation to the camera
        Vector3 direction = new(0, 0, -distanceFromPlayer);
        Quaternion rotationQuat = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        transform.position = orientation.position + offset + rotationQuat * direction;
        transform.LookAt(orientation.position + offset);

        // Rotate the player after the camera has rotated
        orientation.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }
}
