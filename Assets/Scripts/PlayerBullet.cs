using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] float speed;

    Vector3 direction;

    Camera mainCamera;
    Rigidbody2D myRigidbody;

    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        myRigidbody = GetComponent<Rigidbody2D>();

        MoveTowardMouse();
    }

    private void MoveTowardMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        direction = (mousePosition - transform.position).normalized;

        myRigidbody.velocity = new Vector2(direction.x * speed, direction.y * speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this.gameObject, 1f);
        }
    }
}
