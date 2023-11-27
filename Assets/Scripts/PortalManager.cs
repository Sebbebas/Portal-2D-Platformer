using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] GameObject portalPrefab;
    [SerializeField] Color[] portalColors;
    [SerializeField] GameObject[] activePortals;

    private void Update()
    {
        activePortals = GameObject.FindGameObjectsWithTag("Portal");
    }

    public void PortalSpawn(Vector2 pos, int color)
    {
        Instantiate(portalPrefab, pos, Quaternion.identity);
    }
}
