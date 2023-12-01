using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Serialized Fields

    [Header("Movement")]
    [SerializeField, Tooltip("Sebbe vad gör movementSpeed")] float movementSpeed = 9f; 
    [SerializeField] float acceleration = 3f;

    [Header("Jump")]
    [SerializeField, Tooltip("Doesn't affect jump height any higher because fuck u")] float jumpForce = 20f;
    [SerializeField] float jumpFall = 2.5f;
    [SerializeField] float coyoteTime = 0.1f;

    [Header("Dash")]
    [SerializeField, Tooltip("Dashable layers")] LayerMask dashCheckLayers;
    [SerializeField] float dashStrenght = 10f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Grounded")]
    [SerializeField, Tooltip("Jumpable layers")] LayerMask groundCheckLayers;
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
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        Move();
        Flip();
        GroundCheck();
        DashCheck();

        if (isDashing) {return;}
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce * 1.5f);

            if (myRigidbody.velocity.y >= 0f)
            {
                myRigidbody.gravityScale = jumpFall;
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
            StartCoroutine(DashCoroutine());
        }
    }

    private void Move()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed, myRigidbody.velocity.y);
        Vector2 accelerationVector = (playerVelocity - myRigidbody.velocity) * acceleration;

        myRigidbody.AddForce(accelerationVector);

        if(myRigidbody.velocity.magnitude > movementSpeed)
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

    private IEnumerator CoyoteTimeCoroutine()
    {
        isGrounded = true;
        yield return new WaitForSeconds(coyoteTime);
        isGrounded = false;
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
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

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        myRigidbody.gravityScale = originalGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position - groundCheckOffset, groundCheckRadius);
    }
}