using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]//this makes the amount changeable in Unity
    private float movementSpeed = 1.0f;
    
    [SerializeField]//this makes the amount changeable in Unity
    private float jumpSpeed = 5.0f;
    
    [SerializeField]
    private Rigidbody2D playerRigidbody;
    
    [SerializeField]
    private Transform groundcheck;
    
    [SerializeField]
    private float groundCheckRadius = 0.2f;

    [SerializeField]
    private LayerMask groundLayer;
    
    private float knockbackTimer = 0f;

    [SerializeField]
    private float knockbackDuration = 0.3f;
    
    [SerializeField]
    private float jumpBoostDuration = 3f;
   
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpPressed;
    private float jumpBoostTimer = 0f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        horizontalInput = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }
        
        // ↓↓↓ 新增 ↓↓↓
        if (jumpBoostTimer > 0)
        {
            jumpBoostTimer -= Time.deltaTime;
        }
        // ↑↑↑ 新增结束 ↑↑↑
    }

    private void FixedUpdate()
    {
        CheckGround();

        // ↓↓↓ 改动：knockback 期间不覆盖速度 ↓↓↓
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
        }
        else
        {
            Move(horizontalInput);
        }
        // ↑↑↑ 改动结束 ↑↑↑

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }
        
    }

    private void OnDrawGizmos()
    {
        if (groundcheck == null)
        { 
            Debug.Log ("Reference to GroundCheck is missing");
            return;
        }

        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawWireSphere(groundcheck.position, groundCheckRadius);
    }

    private void Move(float movementInput)
    {
        float horizontalVelocity = movementInput * movementSpeed;
        Vector2 movementVelocity = new Vector2(horizontalVelocity, playerRigidbody.linearVelocity.y);
        playerRigidbody.linearVelocity = movementVelocity;
    }

    private void Jump()
    {
        if (isGrounded == false)
        {
            return;
        }
        
        float currentJumpSpeed = jumpBoostTimer > 0 ? jumpSpeed * 2f : jumpSpeed;
        Vector2 jumpVelocity = new Vector2(playerRigidbody.linearVelocityX, jumpSpeed);
        playerRigidbody.linearVelocity = jumpVelocity;
        
    }

    private void CheckGround()
    {
        Collider2D detectedGround = Physics2D.OverlapCircle(groundcheck.position, groundCheckRadius, groundLayer);
        isGrounded = detectedGround != null; //the same as if-else, good for detect, use if-else if the reference is important)
        
    }
    
    // ↓↓↓ 新增方法 ↓↓↓
    public void ApplyKnockback(Vector2 force)
    {
        playerRigidbody.linearVelocity = Vector2.zero;
        playerRigidbody.AddForce(force, ForceMode2D.Impulse);
        knockbackTimer = knockbackDuration;
    }
    // ↑↑↑ 新增结束 ↑↑↑
    
    public void ApplyJumpBoost()
    {
        jumpBoostTimer = jumpBoostDuration;
    }
    
}
