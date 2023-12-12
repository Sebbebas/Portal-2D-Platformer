using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    Vector2 activeSpawnPoint;

    private void Start()
    {
        activeSpawnPoint = FindObjectOfType<PlayerMove>().transform.position;
    }

    public void SetSpawnPoint(Vector2 newPoint) 
    { 
        activeSpawnPoint = newPoint;
    }

    public Vector2 GetSpawnPoint() 
    { 
        return activeSpawnPoint;
    }
}
