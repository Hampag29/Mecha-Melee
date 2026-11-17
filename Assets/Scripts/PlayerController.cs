using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
// adds a header in the player script inspector in unity
    [Header("movement")]  
    // variables used for player movement 
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float dashforce = 6f;
    [SerializeField] private float acceleration = 0.5f;

    [Header("ground check")]
    // varibles used to later to chech if the player is on the ground
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Health")]
    // varibles used for player health
    [SerializeField] public float Health;
    [SerializeField] public float maxHealth;
    [SerializeField] healthBarUI healthBar;
    [Header("attack")]
    // varibles used for player attack
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float DamageAmount = 20f;
    [SerializeField] private float attackTime = 0.15f;

    //variables used for player attack audio
    [SerializeField] public AudioClip attackSound;
    private AudioSource Source;

    //variables used for getting commponents 
    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 movement;
    private bool onGround;
    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    private RaycastHit2D[] hits;
    private float attackTimeCounter;
    private bool isFloating = false;
    public float floatSpeed = 1f;

    private void Awake()
    {
        // setting up all components needed
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Source = GetComponent<AudioSource>();

        //uses new unity input system so that when that action is pressed it calls the coresponding function.
        playerControls.Movement.Jump.performed += OnJump;
        playerControls.Movement.Dash.performed += Dash;
        playerControls.Attack.basic.performed += Attack;
        playerControls.Gravity.Shift.performed += ApplyGravityMode;
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



    // Update is called once per frame
    void Update()
    {
        //sets up movement to take in from the input system
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        //ground check to see if player is on ground
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //updates animations
        updateAnimations();
        //flips sprite if they are moving left or if they are moving right
        flipSprite();

        //sets up an attack cooldown so they can't attack as often as they want
        attackTimeCounter += Time.deltaTime;

    }
    private void FixedUpdate()
    {
 // if the target is not using the gravity mechanic
        if(!isFloating)
        { 
            //code for player movement
        float targetXspeed = Mathf.MoveTowards(rb.linearVelocity.x, movement.x * moveSpeed, acceleration);
        rb.linearVelocity = new Vector2(targetXspeed, rb.linearVelocity.y);
            }
        //if the player health reaches 0 the scene reloads 
        if (Health <=0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        //if the player is on the ground and isn't floating
        if (onGround && !isFloating)
        {
            // jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

    }
    private void Dash(InputAction.CallbackContext context)
    {
        //dash
        rb.linearVelocity = new Vector2(dashforce * (movement.x), 2);
    }
    private void updateAnimations()
    {
        //if the player is moving it triggers the walk animation
        playerAnimator.SetFloat("moveX", movement.x);
        //if the player is in the air it triggers the falling animation
        playerAnimator.SetBool("isGrounded", onGround);

    }
    private void flipSprite()
    {
        //if the player is moving left or right it changes the way the sprite is fliped 
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
    // takes in float when function is called
    public void setHealth(float changeHealth)
    {
        //health is equal to the float taken in
        Health += changeHealth;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        healthBar.setHealth(Health);
    }
    private void Attack(InputAction.CallbackContext context)
    {
        //uses the attack cooldown 
        if (attackTimeCounter >= attackTime)
        {
            attackTimeCounter = 0f;
            // casts an invisable circle infront of the player
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackLayer);
            
            for (int i = 0; i < hits.Length; i++)
            {
                //if the circle hits an enemy
                Enemymech enemyHealth = hits[i].collider.GetComponent<Enemymech>();
                // and the enemy still has health
                if (enemyHealth != null)
                {
                  
                    enemyHealth.damage(DamageAmount);
                }
                // if the attack hits the boss, the boss takes damage too
                Boss boss = hits[i].collider.GetComponent<Boss>();
                if (boss != null) boss.damage(DamageAmount);
            }
            //triggers the attack animation
            playerAnimator.SetTrigger("attack");
            //trigers the attack sound
            Source.PlayOneShot(attackSound, 1.0f);
        }   
    }
   //taken from enemy mech.cs
   private void ApplyGravityMode(InputAction.CallbackContext context)
    {
        isFloating = !isFloating;
        if (isFloating)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized * floatSpeed;
        }
        else
        {
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
