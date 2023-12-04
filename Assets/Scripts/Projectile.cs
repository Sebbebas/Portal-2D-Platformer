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

        Vector2 gubb = transform.right * speed;
        myRigidbody.velocity = gubb;
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();
        if (player == null)
        {
            player.PlayerDamage(20);
        }
        Destroy(gameObject);
    }
}