using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField, Tooltip("Spawn point offset from checkpoint")] Vector2 spawnOffset = new Vector2(-1.5f, 0f);
    [Space]
    [SerializeField, Tooltip("Activation area radius")] float activationRadius = 1.5f;
    [SerializeField, Tooltip("Activation area offset from checkpoint")] Vector2 activationOffset = Vector2.zero;
    [Space]
    [SerializeField] LayerMask activatingLayers;

    bool isActive = false;

    private void Start()
    {
        StartCoroutine(ActivationCheck());
    }

    private IEnumerator ActivationCheck() 
    { 
        while (true) 
        {
            CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
            Vector2 point = (Vector2)transform.position - activationOffset;

            if (Physics2D.OverlapCircle(point, activationRadius, activatingLayers) && !isActive) 
            {
                checkpointManager.SetSpawnPoint((Vector2)transform.position + spawnOffset);
                isActive = true;
                Debug.Log("Checkpoint Activated");
            }

            yield return 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + activationOffset, activationRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + activationOffset);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnOffset, 0.25f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + spawnOffset);
    }
}
