using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private Transform detectionOriginPoint;
    [SerializeField] private float targetSwitchLockoutTime = 5f;
    [SerializeField] private float searchTime = 10f;
    [SerializeField] private float maxChaseTime = 30f;
    [SerializeField] private float detectionCooldownTime = 5f;
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
    public GameObject player;
    private float detectionRange;
    private bool detected = false;
    private float targetSwitchLockoutTimer = 0f;
    public bool isChasing = false;
    private Vector3 lastKnownLocation;
    private bool isSearching = false;
    private int currentPatrolIndex = 0;
    private float chaseStartTime;
    private float detectionCooldownTimer = 0f;
    private bool moveToLastKnownLocation = false;
    private bool isReversing;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        detectionRange = normalDetectionRange;
        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
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
        else if (!isChasing && !isSearching)
        {
            Patroling();
        }

        if (isChasing && Time.time - chaseStartTime > maxChaseTime)
        {
            StopChasing();
        }

        if (moveToLastKnownLocation && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f)
        {
            moveToLastKnownLocation = false;
            Searching();
        }
    }

    private void DetectPlayer()
    {
        if (Time.time < targetSwitchLockoutTimer || Time.time < detectionCooldownTimer)
        {
            return;
        }

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

                            // Stop searching if the player is detected
                            StopSearching();
                            return;
                        }
                    }
                }
            }
        }

        if (player != null)
        {
            lastKnownLocation = player.transform.position;
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
        if (!isSearching)
        {
         StartCoroutine(SearchCoroutine());
        }
    }

    private void StopSearching()
    {
        if (isSearching)
        {
            isSearching = false;
            StopCoroutine(SearchCoroutine());
        }
    }

    private IEnumerator SearchCoroutine()
    {
        isSearching = true;
        float searchEndTime = Time.time + searchTime;

        while (Time.time < searchEndTime)
        {
            // Check for player detection during search
            DetectPlayer();
            if (detected)
            {
                yield break; // Exit the coroutine if the player is detected
            }

            float lookAroundDuration = 2f;
            float lookAroundTimer = 0f;

            while (lookAroundTimer < lookAroundDuration)
            {
                transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
                lookAroundTimer += Time.deltaTime;
                yield return null;
            }
            agent.SetDestination(lastKnownLocation);
            yield return null;
        }

        isSearching = false;
        Patroling();
    }

    private void Patroling()
    {
        if (patrolPoints.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Check if we need to reverse the patrol direction
            if (!isReversing && currentPatrolIndex == patrolPoints.Count - 1)
            {
                isReversing = true; // Start reversing
            }
            else if (isReversing && currentPatrolIndex == 0)
            {
                isReversing = false; // Start moving forward
            }

            // Update the patrol index based on the current direction
            currentPatrolIndex += isReversing ? -1 : 1;

            // Set the next destination
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void Chase()
    {
        if (player == null) return;

        if (!isChasing)
        {
            chaseStartTime = Time.time;
        }

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

        detectionCooldownTimer = Time.time + detectionCooldownTime; // Start cooldown timer

        moveToLastKnownLocation = true;
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

  

#if UNITY_EDITOR
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
#endif
}
