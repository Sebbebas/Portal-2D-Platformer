using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Serialized Fields

    [Header("Movement")]
    [SerializeField, Tooltip("Sebbe vad gör movementSpeed")] float movementSpeed = 12f; 
    [SerializeField] float acceleration = 3f;

    [Header("Jump")]
    [SerializeField, Tooltip("Is capped at movementSpeed")] float jumpForce = 20f;
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

    [Header("Sticky Walls")]
    [SerializeField] LayerMask wallCheckLayer;
    [SerializeField] Vector2 wallCheckOffset;
    [SerializeField] float wallCheckRadius = 1f;

    [Header("On Ice")]
    [SerializeField] LayerMask slipperyCheckLayers;
    [SerializeField] float movementOnIce = 16f;
    [SerializeField] float accelerationOnIce = 6f;

    [Header("Zooming Out")]
    [SerializeField] LayerMask zoomCheckLayers;

    [Space]

    //Cached reference

    [SerializeField] Rigidbody2D myRigidbody;
    [SerializeField] PhysicsMaterial2D frictionPhysics;

    CameraController cameraController;

    // Private variables

    float originalMovement;
    float originalAcceleration;

    bool isFacingRight;
    bool isGrounded;
    bool isWalled;
    bool isSlippery;

    bool canDash;
    bool isDashing;

    Vector2 moveInput;

    private void Awake()
    {
        SetVariables();        
    }

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        cameraController = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        #region calling functions
        Move();
        Flip();
        GroundCheck();
        WallCheck();
        DashCheck();
        SlipperyCheck();
        ZoomCheck();
        #endregion

        if (isDashing) { return; } 
    }

    #region Inputs
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
    #endregion

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

    #region Find Layers
    private void SlipperyCheck()
    {
        Vector2 slipperyCheckPos = (Vector2)transform.position - groundCheckOffset;

        isSlippery = Physics2D.OverlapCircle(slipperyCheckPos, groundCheckRadius, slipperyCheckLayers);

        if (isSlippery == true)
        {
            movementSpeed = movementOnIce;
            acceleration = accelerationOnIce;
        }
        else
        {
            movementSpeed = originalMovement;
            acceleration = originalAcceleration;
        }
    }

    private void GroundCheck()
    {
        Vector2 groundCheckPos = (Vector2)transform.position - groundCheckOffset;

        if (Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundCheckLayers))
        {
            isGrounded = true;
        }
        else
        {
            StartCoroutine(CoyoteTimeCoroutine());
        }
    }

    private void WallCheck()
    {
        Vector2 wallCheckPos = (Vector2)transform.position - wallCheckOffset;

        isWalled = Physics2D.OverlapCircle(wallCheckPos, wallCheckRadius, wallCheckLayer);

        if (isWalled == true)
        {
            myRigidbody.sharedMaterial = frictionPhysics;
        }
    }

    private void DashCheck()
    {
        Vector2 dashCheckPos = (Vector2)transform.position - groundCheckOffset;

        canDash = Physics2D.OverlapCircle(dashCheckPos, groundCheckRadius, dashCheckLayers);
    }

    private void ZoomCheck()
    {
        Vector2 zoomCheckPos = (Vector2)transform.position - groundCheckOffset;

        cameraController.zoomActive = Physics2D.OverlapCircle(zoomCheckPos, groundCheckRadius, zoomCheckLayers);
    }
    #endregion

    #region Coroutines

    public IEnumerator Knockback(Vector2 force, float direction, float duration)
    {
        float elapsedTime = 0;

        while (duration > elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            Vector2 knockback = new Vector2(direction * force.x, force.y); // Direction kan vara transform.right 
            myRigidbody.velocity = knockback;

            yield return null;
        }
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
    #endregion

    private void SetVariables()
    {
        isGrounded = false;
        isWalled = false;
        isSlippery = false;

        originalAcceleration = acceleration;
        originalMovement = movementSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position - groundCheckOffset, groundCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position - wallCheckOffset, wallCheckRadius);
    }
}