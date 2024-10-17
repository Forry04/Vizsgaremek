using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [InspectorLabel("Mouse Sensitivity")]
    [SerializeField] private float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Transform orientation;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawned");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        orientation = transform.parent;
        if (!IsLocalPlayer)
        {
            GetComponent<Camera>().enabled = false;
            return;
        }
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        orientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
