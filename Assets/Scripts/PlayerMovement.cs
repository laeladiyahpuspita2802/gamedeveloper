using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private Vector2 moveInput;
    private bool isJumping = false;
    private bool isRunning = false;
    private bool isAttacking = false;
    private bool isThrowing = false;
    private bool isHit = false;

    private enum MovementState { idle, run, jump, attack, throwObj, hit }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerController = new PlayerController(); // Instantiate PlayerController
    }

    private void OnEnable()
    {
        playerController.Enable(); // Enable PlayerController

        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        playerController.Movement.Jump.performed += ctx => Jump();
        playerController.Movement.Run.performed += ctx => isRunning = true;
        playerController.Movement.Run.canceled += ctx => isRunning = false;

        playerController.Movement.Attack.performed += ctx => StartCoroutine(DoAction("attack"));
        playerController.Movement.Throw.performed += ctx => StartCoroutine(DoAction("throw"));
        playerController.Movement.Hit.performed += ctx => StartCoroutine(DoAction("hit"));
    }

    private void OnDisable()
    {
        playerController.Disable(); // Disable PlayerController
    }

    private void Update()
    {
        moveInput = playerController.Movement.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Determine movement speed based on running or walking
        float speed = isRunning ? runSpeed : moveSpeed;
        Vector2 targetVelocity = new Vector2(moveInput.x * speed, rb.velocity.y);
        rb.velocity = targetVelocity;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (isHit)
        {
            state = MovementState.hit;
        }
        else if (isAttacking)
        {
            state = MovementState.attack;
        }
        else if (isThrowing)
        {
            state = MovementState.throwObj;
        }
        else if (!IsGrounded()) // Player is jumping
        {
            state = MovementState.jump;
        }
        else if (moveInput.x != 0f)
        {
            state = MovementState.run;
            sprite.flipX = moveInput.x < 0f; // Flip sprite depending on direction
        }
        else
        {
            state = MovementState.idle;
        }

        anim.SetInteger("state", (int)state); // Set animation state based on current state
    }

    private bool IsGrounded()
    {
        // Check if the player is grounded using BoxCast
        return Physics2D.BoxCast(
            coll.bounds.center,
            coll.bounds.size,
            0f,
            Vector2.down,
            .1f,
            jumpableGround
        );
    }

    private void Jump()
    {
        if (IsGrounded()) // Only jump if grounded
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
        }
    }

    private IEnumerator DoAction(string actionType)
    {
        switch (actionType)
        {
            case "attack":
                isAttacking = true;
                break;
            case "throw":
                isThrowing = true;
                break;
            case "hit":
                isHit = true;
                break;
        }

        yield return new WaitForSeconds(0.5f); // Adjust this time according to animation length

        isAttacking = false;
        isThrowing = false;
        isHit = false;
    }
}
