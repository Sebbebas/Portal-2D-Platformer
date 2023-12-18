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
    [SerializeField] float coyoteTime = .1f;

    [Header("Dash")]
    [SerializeField, Tooltip("Dashable layers")] LayerMask dashCheckLayers;
    //[SerializeField, Tooltip("Dashable layers")] string dashCheckTag;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = .2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField, Tooltip("Uses this gravity instead of jumpFall when dashing to allow longer dashes")] float dashGravity = 1f;
    [SerializeField, Tooltip("How long it takes for player to come to a stop after dashing")] float dashDeccelerationTime = 0.25f;

    [Header("Grounded")]
    [SerializeField, Tooltip("Layer the player can jump on")] LayerMask groundCheckLayers;
    [SerializeField] Vector2 groundCheckOffset;
    [SerializeField] float groundCheckRadius = 1f;

    [Header("Sticky Walls")]
    [SerializeField, Tooltip("Layers that player can stick to")] LayerMask wallCheckLayer;
    //[SerializeField, Tooltip("Layers that player can stick to")] string wallCheckTag;
    [SerializeField] Vector2 wallCheckOffset;
    [SerializeField] float wallCheckRadius = 1f;

    [Header("On Ice")]
    [SerializeField, Tooltip("Layers were the player gets less control over their movement")] LayerMask slipperyCheckLayers;
    //[SerializeField, Tooltip("Layers were the player gets less control over their movement")] string slipperyCheckTag;
    [SerializeField] float movementOnIce = 16f;
    [SerializeField] float accelerationOnIce = 6f;


    [Header("Zooming Out")]
    [SerializeField, Tooltip("If the player stands on this the camera size will change")] LayerMask zoomCheckLayers;
    //[SerializeField, Tooltip("If the player stands on this the camera size will change")] string zoomCheckTag;

    [Space]

    //Cached reference

    [SerializeField] Rigidbody2D myRigidbody;
    [SerializeField] PhysicsMaterial2D frictionPhysics;
    [SerializeField] PhysicsMaterial2D playerPhysics;

    CameraController cameraController;

    // Private variables

    float originalMovement;
    float originalAcceleration;
    float dashCooldownTime;

    bool isFacingRight;
    bool isGrounded;
    bool isWalled;
    bool isSlippery;

    bool canDash;
    bool isDashing;

    Vector2 moveInput;
    Vector2 latestMoveDirection = new Vector2(1f, 0f);

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

        if(isDashing)
        {
            myRigidbody.gravityScale = dashGravity;
        }
        else
        {
            myRigidbody.gravityScale = jumpFall;
        }

        #region calling functions
        Move();
        Flip();
        GroundCheck();
        WallCheck();
        DashCheck();
        SlipperyCheck();
        ZoomCheck();
        #endregion
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
            Jump();
        }

        if (context.canceled && myRigidbody.velocity.y > 0f)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f);
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started && canDash)
        {
            StartCoroutine("Dash");
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

        if (moveInput.sqrMagnitude > 0.0f)
        {
            latestMoveDirection = moveInput;
        }
    }

    private void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
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
        //isSlippery = Physics2D.OverlapCircle(slipperyCheckPos, groundCheckRadius, LayerMask.GetMask(slipperyCheckTag)) != null;

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
            StartCoroutine("CoyoteTimeCoroutine");
        }
    }

    private void WallCheck()
    {
        Vector2 wallCheckPos = (Vector2)transform.position - wallCheckOffset;

        isWalled = Physics2D.OverlapCircle(wallCheckPos, wallCheckRadius, wallCheckLayer);
        //isWalled = Physics2D.OverlapCircle(wallCheckPos, wallCheckRadius, LayerMask.GetMask(wallCheckTag));

        if (isWalled == true)
        {
            myRigidbody.sharedMaterial = frictionPhysics;
        }
        else
        {
            myRigidbody.sharedMaterial = playerPhysics;
        }
    }

    private void DashCheck()
    {
        Vector2 dashCheckPos = (Vector2)transform.position - groundCheckOffset;

        canDash = Physics2D.OverlapCircle(dashCheckPos, groundCheckRadius, dashCheckLayers);
        //canDash = Physics2D.OverlapCircle(dashCheckPos, groundCheckRadius, LayerMask.GetMask(dashCheckTag));
    }

    private void ZoomCheck()
    {
        Vector2 zoomCheckPos = (Vector2)transform.position - groundCheckOffset;

        cameraController.zoomActive = Physics2D.OverlapCircle(zoomCheckPos, groundCheckRadius, zoomCheckLayers);
        //cameraController.zoomActive = Physics2D.OverlapCircle(zoomCheckPos, groundCheckRadius, LayerMask.GetMask(zoomCheckTag));
    }
    #endregion

    #region Coroutines

    public IEnumerator Knockback(Vector2 force, float direction, float duration)
    {
        float elapsedTime = 0;

        while (duration > elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            Vector2 knockback = new Vector2(direction * force.x, force.y);  
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

    private IEnumerator Dash()
    {
        isDashing = true;
        myRigidbody.velocity = latestMoveDirection * dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        myRigidbody.velocity = Vector2.Lerp(myRigidbody.velocity, Vector2.zero, dashDeccelerationTime);

        dashCooldown = dashCooldownTime;
        isDashing = false;
    }
    #endregion

    private void SetVariables()
    {
        isGrounded = false;
        isWalled = false;
        isSlippery = false;
        isDashing = false;

        dashCooldownTime = dashCooldown;

        originalAcceleration = acceleration;
        originalMovement = movementSpeed;

        myRigidbody.gravityScale = jumpFall;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position - groundCheckOffset, groundCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position - wallCheckOffset, wallCheckRadius);
    }
}