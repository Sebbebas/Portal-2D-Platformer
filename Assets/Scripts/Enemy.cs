using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float shootingInterval = 1f;
    [SerializeField] float detectionRange = 5f;

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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FindObjectOfType<Playermovement>().Knockback(new Vector2(20.0f, 5.0f), 0.5f));
        }
    }
}