using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform detectionOriginPoint;
    [SerializeField] private float normalDetectionRange = 10f;
    [SerializeField] private float detectedDetectionRangeModifier = 1.5f;
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private float Speed = 3.5f;
    [SerializeField] private float detectedSpeedModifier = 1.2f;
    [SerializeField] private float chaseLockTime = 10f;

    private NavMeshAgent agent;
    private GameObject player;
    private float detectionRange;
    private bool detected = false;
    private float chaseLockTimer = 0f;
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
                    RaycastHit raycastHit;
                    if (Physics.Raycast(detectionOriginPoint.position, directionToPlayer, out raycastHit, detectionRange))
                    {
                        if (raycastHit.collider.CompareTag("Player"))
                        {
                            player = raycastHit.collider.gameObject;
                            detected = true;
                            lastKnownLocation = player.transform.position;
                            return;
                        }
                    }
                }
            }
        }

        detected = false;
        player = null;
    }

    private void Chase()
    {
        if (player == null) return;

        detectionRange = normalDetectionRange * detectedDetectionRangeModifier;
        agent.speed = Speed * detectedSpeedModifier;
        agent.SetDestination(player.transform.position);
        chaseLockTimer = Time.time + chaseLockTime;
        isChasing = true;
    }

    private void StopChasing()
    {
        isChasing = false;
        agent.speed = Speed;
        detectionRange = normalDetectionRange;
      
    }

    private void OnDrawGizmos()
    {
        if (detectionOriginPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(detectionOriginPoint.position, detectionRange);

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
                        }
                    }
                }
            }
        }
    }
}


