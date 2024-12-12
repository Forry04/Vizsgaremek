using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private Transform detectionOriginPoint;
    [SerializeField] private float targetSwitchLockoutTime = 10f;
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float turnSpeed = 120f;
    [Header("Detection Settings")]
    [SerializeField] private float proxyDetectionRange = 2f;
    [SerializeField] private float normalDetectionRange = 10f;
    [Range(0, 360)]
    [SerializeField] private float detectionAngle = 80f;
    [Range(1f, 3f)]
    [SerializeField] private float detectedDetectionRangeModifier = 1.5f;
    [Range(1f, 2f)]
    [SerializeField] private float detectedSpeedModifier = 1.2f;

    private NavMeshAgent agent;
    private GameObject player;
    private float detectionRange;
    private bool detected = false;
    private float targetSwitchLockoutTimer = 0f;
    private bool isChasing = false;
    private Vector3 lastKnownLocation;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        detectionRange = normalDetectionRange;

    }

    private void Update()
    {
        DetectPlayer();
        ProxyDetection();

        if (detected)
        {
            Chase();
        }
        else if (!detected && isChasing)
        {
            StopChasing();
        }
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(detectionOriginPoint.position, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (hit.transform.position - detectionOriginPoint.position).normalized;
                float angleToPlayer = Vector3.Angle(detectionOriginPoint.forward, directionToPlayer);

                if (angleToPlayer < detectionAngle / 2)
                {
                    if (Physics.Raycast(detectionOriginPoint.position, directionToPlayer, out RaycastHit raycastHit, detectionRange))
                    {
                        if (raycastHit.collider.CompareTag("Player"))
                        {
                            targetSwitchLockoutTimer = Time.time + targetSwitchLockoutTime;
                            player = raycastHit.collider.gameObject;
                            detected = true;
                            lastKnownLocation = raycastHit.point;
                            return;
                        }
                    }
                }
            }
        }

        detected = false;
        player = null;
    }

    private void ProxyDetection()
    {
        
        Collider[] hits = Physics.OverlapSphere(transform.position, proxyDetectionRange);
        foreach (var hit in hits)
        {
   
            if (hit.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (hit.transform.position - transform.position).normalized;

                
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit raycastHit, proxyDetectionRange))
                {
                    
                    if (raycastHit.collider.CompareTag("Player") && raycastHit.collider.gameObject != gameObject)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToPlayer), turnSpeed * Time.deltaTime);
                        Debug.Log("Player detected by proxy");
                    }
                }
            }
        }
    }

    private void Searching()
    {
        throw new NotImplementedException();
    }

    private void Patroling()
    {
        throw new NotImplementedException();
    }

    private void Chase()
    {
        if (player == null) return;

        detectionRange = normalDetectionRange * detectedDetectionRangeModifier;
        agent.speed = speed * detectedSpeedModifier;
        agent.SetDestination(player.transform.position);
        isChasing = true;
    }

    private void StopChasing()
    {
        isChasing = false;
        agent.speed = speed;
        detectionRange = normalDetectionRange;
        agent.SetDestination(lastKnownLocation);
       
    }

    public void TriggerEnemyRemotly()
    {
        throw new NotImplementedException();

    }
    private void CheckIfDestinationReached()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !agent.hasPath && agent.velocity.sqrMagnitude == 0f)
        {
            throw new NotImplementedException();
        }
    }


            // Debug
    private void OnDrawGizmos()
    {
        // Draw the destination point for the enemy
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(agent.destination, 0.5f);
        }

        if (detectionOriginPoint != null)
        {
            // Draw the normal detection range
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(detectionOriginPoint.position, Vector3.up, detectionRange);

            // Draw the proxy detection range
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, proxyDetectionRange);

            Vector3 startAngle = Quaternion.AngleAxis(detectionAngle / 2, Vector3.up) * detectionOriginPoint.forward;
            Vector3 endAngle = Quaternion.AngleAxis(-detectionAngle / 2, Vector3.up) * detectionOriginPoint.forward;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(detectionOriginPoint.position, startAngle * detectionRange);
            Gizmos.DrawRay(detectionOriginPoint.position, endAngle * detectionRange);

            Collider[] hits = Physics.OverlapSphere(detectionOriginPoint.position, detectionRange);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Vector3 directionToPlayer = (hit.transform.position - detectionOriginPoint.position).normalized;
                    float angleToPlayer = Vector3.Angle(detectionOriginPoint.forward, directionToPlayer);

                    if (angleToPlayer < detectionAngle / 2)
                    {
                        RaycastHit raycastHit;
                        Gizmos.color = Color.green;
                        Gizmos.DrawRay(detectionOriginPoint.position, directionToPlayer * detectionRange);
                        if (Physics.Raycast(detectionOriginPoint.position, directionToPlayer, out raycastHit, detectionRange))
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawLine(detectionOriginPoint.position, raycastHit.point);
                            if (raycastHit.collider.gameObject == player)
                            {
                                Gizmos.color = Color.yellow;
                                Gizmos.DrawSphere(player.transform.position + Vector3.up * 2, 0.5f);
                            }
                        }
                    }
                }
            }
        }
    }
}