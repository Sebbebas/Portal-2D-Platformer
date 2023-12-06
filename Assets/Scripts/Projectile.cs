using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] int damage = 20;
    
    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        Vector2 move = transform.right * speed;
        myRigidbody.velocity = move;
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();
        if (player == null)
        {
            FindObjectOfType<PlayerHealth>().PlayerDamage(20);
            StartCoroutine(FindObjectOfType<PlayerMove>().Knockback(new Vector2(50f, 5f), transform.right.x, 50f));
        }
        Destroy(gameObject);
    }
}