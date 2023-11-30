using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject deathEffect;
    [SerializeField] float speed = 2f;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float shootingInterval = 1f;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] int health = 100;
   

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("ShootProjectile", speed, shootingInterval);
    }

    void ShootProjectile()
    {
        if (player == null)
            return;

        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= detectionRange)
        {
            GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
            proj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

            //Get EnemyWeapon and call the Shoot method
            EnemyWeapon enemyWeapon = GetComponent<EnemyWeapon>();
            if (enemyWeapon != null)
            {
                enemyWeapon.Shoot();
            }
        }
    }

    public void TakeDamage (int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
    //Dö
    void Die ()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}