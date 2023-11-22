using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    //Configurabel Perameters
    [Header("Movement")]
    [SerializeField] float movementSpeed = 8f;
    [SerializeField] float jumpForce = 12f;

    [Header("Grounded")]
    [SerializeField] LayerMask groundCheckLayers;
    [SerializeField] Vector2 groundCheckOffset;
    [SerializeField] float groundCheckRadius = 1f;

    //Private Variabels
    private Vector2 movementInput;
    private bool isGrounded;

    //Cached references
    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        isGrounded = false;
    }

    private void FixedUpdate()
    {
        Move();
        GroundCheck();
        Flip();
    }

    #region Movement
    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
        movementInput.x = Mathf.RoundToInt(movementInput.x);
    }

    void Move()
    {
        float XVelocity = movementInput.x * movementSpeed;
        myRigidbody.velocity = new(Mathf.Lerp(myRigidbody.velocity.x, XVelocity, Time.deltaTime), myRigidbody.velocity.y);
    }

    void Flip()
    {
        if (Mathf.Abs(myRigidbody.velocity.x) < Mathf.Epsilon)
        {
            return;
        }

        transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1);
    }
    #endregion

    #region Jump
    public void OnJump()
    {
        if (!isGrounded) { return; }
        myRigidbody.velocity = new(myRigidbody.velocity.x, jumpForce);
    }

    public void GroundCheck()
    {
        if(Physics2D.OverlapCircle((Vector2)transform.position - groundCheckOffset, groundCheckRadius, groundCheckLayers)) 
        { 
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    #endregion


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position - groundCheckOffset, groundCheckRadius);
    }
}