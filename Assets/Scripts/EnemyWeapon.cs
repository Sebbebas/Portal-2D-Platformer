using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject projectile;

    public void Shoot()
    {
        Instantiate(projectile, firePoint.position, firePoint.rotation);
    }
}