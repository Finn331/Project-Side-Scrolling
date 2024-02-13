using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator anim;

    private float horizontalInput;
    public float moveSpeed;
    public float sprintSpeed;
    public float jumpForce;
    private bool isFacingRight = true;
    private float currentSpeed;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSprinting = false;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;

    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        horizontalInput = Input.GetAxis("Horizontal");
        currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        Vector2 moveDirection = new Vector2(horizontalInput, 0);
        rb.velocity = new Vector2(moveDirection.x * currentSpeed, rb.velocity.y);
        Flip();

        // Wall Checking
        if (IsFacingWall())
        {
            // If facing a wall, play the idle animation
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
            //footstepSound.SetActive(false);
        }
        else
        {
            // Sprinting
            if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(horizontalInput) > 0.1f)
            {
                isSprinting = true;
                anim.SetBool("isRunning", true);
                anim.SetBool("isWalking", false);
                //footstepSound.SetActive(true);
            }
            else
            {
                isSprinting = false;
                anim.SetBool("isRunning", false);

                // Walking
                if (Mathf.Abs(horizontalInput) > 0.1f)
                {
                    anim.SetBool("isWalking", true);
                    //footstepSound.SetActive(true);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    //footstepSound.SetActive(false);
                }
            }

        }

        // Jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool IsFacingWall()
    {
        float direction = isFacingRight ? 1f : -1f;
        Vector2 rayOrigin = new Vector2(wallCheck.position.x, wallCheck.position.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, wallCheckDistance, groundLayer);

        // Return true if there's a wall in front
        return hit.collider != null;
    }

}
