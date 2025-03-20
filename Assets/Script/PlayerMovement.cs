using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    private float moveInput;
    private bool facingRight = true;

    [Header("Jumping")]
    public float jumpForce = 14f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Wall Mechanics")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 12f;
    public Vector2 wallJumpDirection = new Vector2(1, 1.5f);
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpCooldown = 0.2f;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private bool canDash = true;
    private bool isDashing;

    private TrailRenderer trail;

    [Header("Ground & Wall Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform wallCheck;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;

    private Animator animator; // Reference to Animator

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component
        trail = GetComponent<TrailRenderer>();
        trail.enabled = false;
    }

    public float GetFacingDirection()
    {
        if(isWallSliding == true)
        {
            return facingRight ? -1 : 1;
        }
        else
        {
            return facingRight ? 1f : -1f;
        }
       
    }

    private void Update()
    {
        // Ground & Wall Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, groundLayer);

        // Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jumping
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isWallJumping)
        {
            Jump();
            jumpBufferCounter = 0f;
        }

        // Wall Sliding
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }

        // Wall Jumping
        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            WallJump();
        }

        // Dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Update Animator Parameters
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("isJumping", rb.velocity.y > 0);
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        // Movement Handling
        moveInput = Input.GetAxisRaw("Horizontal");
        float targetSpeed = moveInput * moveSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movementForce = speedDifference * accelerationRate;
        rb.AddForce(Vector2.right * movementForce);

        // Flip Player
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("Jump"); // Play jump animation
        coyoteTimeCounter = 0f;
    }

    private void WallJump()
    {
        isWallJumping = true;
        Invoke(nameof(ResetWallJump), wallJumpCooldown);

        Vector2 force = new Vector2(-wallJumpDirection.x * moveSpeed, wallJumpDirection.y * wallJumpForce);
        rb.velocity = force;
        animator.SetTrigger("Jump");
    }

    private void ResetWallJump()
    {
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        trail.enabled = true;

        float dashDirection = facingRight ? 1f : -1f;
        if (moveInput != 0)
        {
            dashDirection = moveInput;
        }

        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        yield return new WaitForSeconds(dashTime);

        isDashing = false;
        trail.enabled = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
