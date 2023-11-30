using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] int damage = 20;
    protected Rigidbody2D rb;

    private void Start(Vector2 vector2)
    {
        Vector2 gubb = transform.right * speed;
        rb.velocity = gubb;
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