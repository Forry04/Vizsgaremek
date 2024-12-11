using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float damping;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float zoomAggressiveness = 0.75f;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private LayerMask terrainLayers;
    [SerializeField] private LayerMask boundaryLayers;

    public Transform target;

    private Vector3 vel = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        Quaternion targetRotation = target.rotation * Quaternion.Euler(rotation);
        Vector3 desiredPosition = targetPosition;

        RaycastHit hit;

        if (Physics.Raycast(target.position, (transform.position - target.position).normalized, out hit, offset.magnitude, obstacleLayers | terrainLayers))
        {
            float distance = Vector3.Distance(target.position, hit.point);
            float zoomFactor = Mathf.Clamp01(distance / offset.magnitude) * zoomAggressiveness;
            desiredPosition = target.position + (offset.normalized * Mathf.Max(minDistance, distance * zoomFactor));
        }

        // Clamp the desired position within the defined boundaries
        if (Physics.Raycast(target.position, (desiredPosition - target.position).normalized, out hit, Vector3.Distance(target.position, desiredPosition), boundaryLayers))
        {
            desiredPosition = hit.point;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref vel, damping);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
    }
}
