using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("movement")] // adds a header in the player script inspector in unity 
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float dashforce = 6f;
    [SerializeField] private float dashDuration = 6f;
    [SerializeField] private float acceleration = 0.5f;

    [Header("ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 movement;
    private bool onGround;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();

        playerControls.Movement.Jump.performed += OnJump;
        playerControls.Movement.Dash.performed += Dash;
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
}
