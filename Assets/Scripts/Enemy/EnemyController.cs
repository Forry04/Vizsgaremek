using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform detectionOriginPoint;
    [SerializeField] private float proxyDetectionRange = 5f;
    [SerializeField] private float normalDetectionRange = 10f;
    [SerializeField] private float detectedDetectionRangeModifier = 1.5f;
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float detectedSpeedModifier = 1.2f;
    [SerializeField] private float targetSwitchLockoutTime = 10f;

    private NavMeshAgent agent;
    private GameObject player;
    private float detectionRange;
    private bool detected = false;
    private float targetSwitchLockoutTimer = 0f;
    private bool isChasing = false;

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
                    if (Physics.Raycast(detectionOriginPoint.position, directionToPlayer, out RaycastHit raycastHit, detectionRange))
                    {
                        if (raycastHit.collider.CompareTag("Player"))
                        {
                            targetSwitchLockoutTimer = Time.time + targetSwitchLockoutTime;
                            player = raycastHit.collider.gameObject;
                            detected = true;
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
        Collider[] hits = Physics.OverlapSphere(detectionOriginPoint.position, proxyDetectionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (hit.transform.position - detectionOriginPoint.position).normalized;
                float angleToPlayer = Vector3.Angle(detectionOriginPoint.forward, directionToPlayer);

                if (angleToPlayer < detectionAngle / 2)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToPlayer), turnSpeed * Time.deltaTime);
                }
            }
        }


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
        agent.ResetPath();

    }

    //Debug
    private void OnDrawGizmos()
    {

        if (detectionOriginPoint != null)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(detectionOriginPoint.position, Vector3.up, detectionRange);

            
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


