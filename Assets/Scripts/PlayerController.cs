using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("movement")] // adds a header in the player script inspector in unity 
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float dashforce = 6f;
    [SerializeField] private float acceleration = 0.5f;

    [Header("ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .2f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Health")]
    [SerializeField] public float Health;
    [SerializeField] public float maxHealth;
    [SerializeField] healthBarUI healthBar;
    [Header("attack")]
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float DamageAmount = 20f;
    [SerializeField] private float attackTime = 0.15f;

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 movement;
    private bool onGround;
    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    private RaycastHit2D[] hits;
    private float attackTimeCounter;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerControls.Movement.Jump.performed += OnJump;
        playerControls.Movement.Dash.performed += Dash;
        playerControls.Attack.basic.performed += Attack;
        healthBar.setMaxHealth(maxHealth);
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        playerControls.Attack.basic.performed += Attack;
        updateAnimations();
        flipSprite();

        // used as a test to see if healthbar is working will delete later 
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            setHealth(-20f);
        }
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            setHealth(20f);
        }
        attackTimeCounter += Time.deltaTime;

    }
    private void FixedUpdate()
    {
        //rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        float targetXspeed = Mathf.MoveTowards(rb.linearVelocity.x, movement.x * moveSpeed, acceleration);
        rb.linearVelocity = new Vector2(targetXspeed, rb.linearVelocity.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

    }
    private void Dash(InputAction.CallbackContext context)
    {
        rb.linearVelocity = new Vector2(dashforce * (movement.x), 2);
    }
    private void updateAnimations()
    {
        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetBool("isGrounded", onGround);

    }
    private void flipSprite()
    {
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
            attackTransform.localPosition = new Vector2(-Mathf.Abs(attackTransform.localPosition.x), attackTransform.localPosition.y);
        }
        else if (movement.x > 0)
        {
        spriteRenderer.flipX = false;
        attackTransform.localPosition = new Vector2(Mathf.Abs(attackTransform.localPosition.x), attackTransform.localPosition.y);
        }
    }
    public void setHealth(float changeHealth)
    {
        Health += changeHealth;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        healthBar.setHealth(Health);
    }
    private void Attack(InputAction.CallbackContext context)
    {
        if (attackTimeCounter >= attackTime)
        {
            attackTimeCounter = 0f;
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                Enemymech enemyHealth = hits[i].collider.GetComponent<Enemymech>();
                if (enemyHealth != null)
                {
                    //damage
                    enemyHealth.damage(DamageAmount);
                }
            }
            playerAnimator.SetTrigger("attack");
        }   
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}
