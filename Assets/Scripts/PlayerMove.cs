using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMove : MonoBehaviour
{
    // Serialized Fields

    [Header("Movement")]
    [SerializeField] float movementSpeed = 9f; //Vad gör den här
    [SerializeField] float acceleration = 3f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpFall = 2.5f;
    [SerializeField] float coyoteTime = 0.1f;

    [Header("Dash")]
    [SerializeField] LayerMask dashCheckLayers;
    [SerializeField] LayerMask dashObstacles;
    [SerializeField] float dashStrenght = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 0.2f;

    [Header("Grounded")]
    [SerializeField] LayerMask groundCheckLayers;
    [SerializeField] Vector2 groundCheckOffset;
    [SerializeField] float groundCheckRadius = 1f;

    [Space]

    [SerializeField] Rigidbody2D myRigidbody;

    // Private variables

    bool isFacingRight;
    bool isGrounded;
    bool canDash;
    bool isDashing;

    Vector2 moveInput;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        isGrounded = false;
    }

    private void Update()
    {
        Move();
        Flip();
        GroundCheck();
        DashCheck();

        if (!isDashing)
        {
            CheckForDashInput();
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);

            if (myRigidbody.velocity.y >= 0f)
            {
                myRigidbody.gravityScale = jumpFall * 1.5f;
            }
        }

        if (context.canceled && myRigidbody.velocity.y > 0f)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f);
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            StartCoroutine(DashCoroutine(dashDirection));
        }
    }

    private void Move()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed, myRigidbody.velocity.y);
        Vector2 accelerationVector = (playerVelocity - myRigidbody.velocity) * acceleration;

        myRigidbody.AddForce(accelerationVector);

        if (myRigidbody.velocity.magnitude > movementSpeed)
        {
            myRigidbody.velocity = myRigidbody.velocity.normalized * movementSpeed;
        }
    }

    private void Flip()
    {
        if ((!isFacingRight && moveInput.x > 0f) || (isFacingRight && moveInput.x < 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void GroundCheck()
    {
        Vector2 groundCheckPosition = (Vector2)transform.position - groundCheckOffset;

        if (Physics2D.OverlapCircle(groundCheckPosition, groundCheckRadius, groundCheckLayers))
        {
            isGrounded = true;
        }
        else
        {
            StartCoroutine(CoyoteTimeCoroutine());
        }
    }

    private void DashCheck()
    {
        Vector2 dashCheckPosition = (Vector2)transform.position - groundCheckOffset;

        canDash = Physics2D.OverlapCircle(dashCheckPosition, groundCheckRadius, dashCheckLayers);
    }

    private void CheckForDashInput()
    {
        /*float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            Vector2 dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
            StartCoroutine(DashCoroutine(dashDirection));
        }*/
    }

    private IEnumerator CoyoteTimeCoroutine()
    {
        isGrounded = true;
        yield return new WaitForSeconds(coyoteTime);
        isGrounded = false;
    }

    private IEnumerator DashCoroutine(Vector2 dashDirection)
    {
        /*canDash = false;
        isDashing = true;
        float originalGravity = myRigidbody.gravityScale;
        myRigidbody.gravityScale = 0f;
        Vector2 direction = new Vector2(moveInput.x, moveInput.y);

        if (direction == Vector2.zero)
        {
            direction = new Vector2(transform.localScale.x, 0f);
        }

        myRigidbody.velocity = direction.normalized * dashStrenght;

        yield return new WaitForSeconds(dashDuration);
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f);
        myRigidbody.gravityScale = originalGravity * 1.5f;

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        myRigidbody.gravityScale = originalGravity;*/

        isDashing = true;

        Vector2 startPosition = myRigidbody.position;
        Vector2 endPosition = startPosition + dashDirection * dashStrenght;

        RaycastHit2D hit = Physics2D.Linecast(startPosition, endPosition, dashObstacles);

        if (hit.collider != null)
        {
            endPosition = hit.point - dashDirection * 0.1f;
        }

        float dashTimer = 0f;

        while (dashTimer < dashCooldown)
        {
            myRigidbody.MovePosition(Vector2.Lerp(startPosition, endPosition, dashTimer / dashDuration));
            dashTimer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position - groundCheckOffset, groundCheckRadius);
    }
}