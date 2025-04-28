using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    public float detectionDistance = 5.0f; // Distance to check for obstacles
    public float enemyDetectionRadius = 5.0f; // Radius to check for enemies
    public LayerMask obstacleLayer; // Layer to identify obstacles
    public LayerMask enemyLayer; // Layer to identify enemies
    public GameSceneSpawner gameSceneSpawner;
    CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEnemyNear() && IsPathClearToEnemy())
        {
            characterController.enabled = false; // Disable character controller
            gameObject.transform.SetPositionAndRotation(gameSceneSpawner.spawnPoints[0].position, gameSceneSpawner.spawnPoints[0].rotation);
            characterController.enabled = true; // Re-enable character controller

        }
    }

    bool IsEnemyNear()
    {
        // Check for colliders within the detection radius on the enemy layer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius, enemyLayer);

        // Return true if any enemy is found
        return hitColliders.Length > 0;
    }

    bool IsPathClearToEnemy()
    {
        // Find the nearest enemy
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius, enemyLayer);
        foreach (Collider enemy in hitColliders)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // Cast a ray toward the enemy to check for obstacles
            if (!Physics.Raycast(transform.position, directionToEnemy, distanceToEnemy, obstacleLayer))
            {
                return true; // Path is clear to at least one enemy
            }
        }

        return false; // No clear path to any enemy
    }

#if UNITY_EDITOR
    // Optional: Draw the detection radius and ray in the Scene view for debugging
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(transform.position, transform.forward * detectionDistance);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, enemyDetectionRadius);
    //}
#endif
}
