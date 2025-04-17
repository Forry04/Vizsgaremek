using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private bool isFirstPerson = false;
    [SerializeField] private GameObject man;

    public SettingsManager Settings => SettingsManager.Instance;

    //[InspectorLabel("Mouse Sensitivity")]
    //[Range(1, 100)]
    //[SerializeField] private float mouseSensitivity = 18f;
    //[Range(1, 500)]
    //[InspectorLabel("Joystick Sensitivity")]

    //[SerializeField] private float joystickSensitivity = 250f;

    [Header("ThirdPersonSettings")]
    [SerializeField] private float distanceFromPlayer = 5f;
    [SerializeField] private Vector3 offset = new(0, 2, 0);
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private float minCameraDistance = 1f;
    [SerializeField] private float crouchHeightAdjustment = -1f;

    [Header("FirstPersonSettings")]
    [SerializeField] private Transform firstPersonCameraPosition;


    private (float X, float Y) rotation = (0, 0);
    private Transform orientation;
    private PlayerInputHandler inputHandler;
    private PlayerMovementController movementController;
    private Vector2 currentRotation;
    private Vector2 rotationVelocity;
    private Vector3 positionVelocity;
    private bool switchCameraTriggered = false;


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
        movementController = transform.parent.GetComponent<PlayerMovementController>();
    }

    private void FixedUpdate()
    {
        if (inputHandler.SwitchCameraTriggered && !switchCameraTriggered)
        {
            SwitchCamera();
            switchCameraTriggered = true; // Set the flag to true after switching the camera
        }
        else if (!inputHandler.SwitchCameraTriggered)
        {
            switchCameraTriggered = false; // Reset the flag when the input is released
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        RotateNametags();
        if (isFirstPerson)
        {
           
            HandleCamMovementFirstPerson();
        }
        else
        {
            HandleCamMovementThirdPreson();

        }
    }
    
    private void SwitchCamera()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        if (isFirstPerson)
        {
            man.layer = LayerMask.NameToLayer("Player");
            transform.SetPositionAndRotation(orientation.position + offset, orientation.rotation);
            transform.localRotation = Quaternion.Euler(rotation.X, 0f, 0f);
            orientation.localRotation = Quaternion.Euler(0f, rotation.Y, 0f);
            Debug.Log("Third person");
        }
        else
        {
            man.layer = LayerMask.NameToLayer("OwnPlayer");
            transform.SetPositionAndRotation(firstPersonCameraPosition.position, firstPersonCameraPosition.rotation);
            Debug.Log("First person");
        }
        isFirstPerson = !isFirstPerson;
    }
    #region firstPerson
    

    private void HandleCamMovementFirstPerson()
    {
        // Get input for camera rotation
        float inputX = inputHandler.LookInput.x * (inputHandler.LookDevice ? Settings.CurrentSettings.mouseSensitivity : Settings.CurrentSettings.gamepadSensitivity) * Time.deltaTime;
        float inputY = inputHandler.LookInput.y * (inputHandler.LookDevice ? Settings.CurrentSettings.mouseSensitivity : Settings.CurrentSettings.gamepadSensitivity) * Time.deltaTime;

        // Update rotation values
        if (Settings.CurrentSettings.invertXAxis) rotation.X += inputY;   
        else rotation.X -= inputY;

        rotation.X = Mathf.Clamp(rotation.X, -75f, 90f);

        if (Settings.CurrentSettings.invertYAxis) rotation.Y -= inputX;
        else rotation.Y += inputX;

        // Limit vertical rotation
       

        // Apply rotation to the camera
        transform.localRotation = Quaternion.Euler(rotation.X, 0f, 0f);
        orientation.localRotation = Quaternion.Euler(0f, rotation.Y, 0f);

        // Smoothly interpolate the camera's position
        Vector3 targetPosition = firstPersonCameraPosition.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, rotationSmoothTime);
    }
    #endregion

    #region thirdPerson
    private void HandleCamMovementThirdPreson()
    {
        float inputX = inputHandler.LookInput.x * (inputHandler.LookDevice ? Settings.CurrentSettings.mouseSensitivity : Settings.CurrentSettings.gamepadSensitivity) * Time.deltaTime;
        float inputY = inputHandler.LookInput.y * (inputHandler.LookDevice ? Settings.CurrentSettings.mouseSensitivity : Settings.CurrentSettings.gamepadSensitivity) * Time.deltaTime;

        if (Settings.CurrentSettings.invertXAxis) rotation.X += inputY;
        else rotation.X -= inputY;

        rotation.X = Mathf.Clamp(rotation.X, -75f, 90f);

        if (Settings.CurrentSettings.invertYAxis) rotation.Y -= inputX;
        else rotation.Y += inputX;

        currentRotation = Vector2.SmoothDamp(currentRotation, new Vector2(rotation.X, rotation.Y), ref rotationVelocity, rotationSmoothTime);

        Vector3 direction = new(0, 0, -distanceFromPlayer);
        Quaternion rotationQuat = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        Vector3 targetPosition = orientation.position + offset + rotationQuat * direction;

        targetPosition = CameraCollisions(targetPosition);

        transform.position = targetPosition;

        Vector3 lookAtPosition = orientation.position + offset;
        if (movementController.IsCrouching)
        {
            lookAtPosition.y += crouchHeightAdjustment;
        }
        transform.LookAt(lookAtPosition);

        orientation.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }

    private Vector3 CameraCollisions(Vector3 targetPosition)
    {
        Vector3 adjustedOffset = offset;
        if (movementController.IsCrouching)
        {
            adjustedOffset.y += crouchHeightAdjustment;
        }

        Vector3 direction = targetPosition - (orientation.position + adjustedOffset);
        if (Physics.SphereCast(orientation.position + adjustedOffset, cameraCollisionRadius, direction, out RaycastHit hit, distanceFromPlayer))
        {
            float distanceToHit = Vector3.Distance(orientation.position + adjustedOffset, hit.point);
            if (distanceToHit < minCameraDistance)
            {
                return orientation.position + adjustedOffset - direction.normalized * minCameraDistance;
            }
            return hit.point + hit.normal * cameraCollisionRadius;
        }
        return targetPosition;
    }
    #endregion
    
    private void RotateNametags()
    {
        GameObject[]  playerNames = GameObject.FindGameObjectsWithTag("PlayerName");
        foreach (GameObject playerName in playerNames)
        {
            if (playerName.transform.parent != null)
            {
                playerName.transform.LookAt(transform.position);
                playerName.transform.Rotate(0, 180, 0);
            }
        }
    }

}
