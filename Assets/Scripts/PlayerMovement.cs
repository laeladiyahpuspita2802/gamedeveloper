using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;
    public TextMeshProUGUI healthText;

    [Header("Knockback Settings")]
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private float knockBackThrust = 10f;

    private bool isKnockedBack = false;

    [Header("Player Movement")]

    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private Vector2 moveInput;
    private float mobileInputX = 0f;

    private bool isRunning = false;
    private bool isJumping = false;
    private bool isAttacking = false;
    private bool isThrowing = false;
    private bool isHit = false;

    private enum MovementState { idle, walk, run, jump, fall, attack, throwObj, hit }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerController = new PlayerController();

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void OnEnable()
    {
        playerController.Enable();

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
        playerController.Disable();
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileInputX, 0f);
        }
        else
        {
            moveInput = playerController.Movement.Move.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return; //false ketika terkena knockback

        float inputX = moveInput.x + mobileInputX;
        float speed = isRunning ? runSpeed : moveSpeed;
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);

        UpdateAnimation();

        if (IsGrounded() && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            isJumping = false;
        }
    }

    private void UpdateAnimation()
    {
        MovementState state;
        float horizontal = moveInput.x != 0 ? moveInput.x : mobileInputX;

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
        else if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.fall;
        }
        else if (horizontal != 0f)
        {
            state = isRunning ? MovementState.run : MovementState.walk;
            sprite.flipX = horizontal < 0f;
        }
        else
        {
            state = MovementState.idle;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
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

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        isThrowing = false;
        isHit = false;
    }

    // Mobile UI Button Methods
    public void MoveRight(bool isPressed)
    {
        if (isPressed)
            mobileInputX = 1f;
        else if (mobileInputX == 1f)
            mobileInputX = 0f;
    }

    public void MoveLeft(bool isPressed)
    {
        if (isPressed)
            mobileInputX = -1f;
        else if (mobileInputX == -1f)
            mobileInputX = 0f;
    }

    public void MobileJump()
    {
        Jump();
    }

    public void TakeDamage(int damage, Vector2 direction)
    {
        if (isKnockedBack) return; // Jangan stack knockback

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player Mati");
        }

        StartCoroutine(HandleKnockback(direction.normalized));
        UpdateHealthUI();
    }

private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    private IEnumerator HandleKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;

        Vector2 force = direction * knockBackThrust * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}
