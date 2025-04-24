using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    [SerializeField] private float collectionRadius = 2f; 
    [SerializeField] private string collectibleTag = "Collectible"; 

    private void Update()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collectionRadius);

        foreach (var hitCollider in hitColliders)
        {   
            if (hitCollider.CompareTag(collectibleTag))
            {
                if (hitCollider.TryGetComponent<ICollectible>(out ICollectible collectible))
                {
                    collectible.Collect(PlayerDataManager.Singleton);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRadius);
    }
#endif
}
