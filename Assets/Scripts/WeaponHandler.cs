using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons
{
    None = 0,
    portalGun,
    gun
}

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] Weapons healdWeapon;
    [SerializeField] float range = 100f;
    [SerializeField] LayerMask layers;

    Camera cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    private void FixedUpdate()
    {
        Vector3 cameraDirection = transform.position - cam.transform.position;
        Physics2D.Raycast(transform.position, cameraDirection, range, layers);
    }
}
