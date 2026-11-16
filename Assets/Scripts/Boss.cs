using UnityEngine;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    //health (boss has 250, no respawn)
    [SerializeField] private float maxHealth = 250f;
    private float currentHealth;

    //gravity mechanic
    public float floatSpeed = 1f;
    private bool isFloating = false;

    //shiver: small front and back movement
    public float shiverSpeed = 10f;   // how fast it shakes back and forth
    public float shiverAmount = 1f;   // how strong the shake is

    //shooting
    // taking the bullet prefab and adding to the code
    public GameObject bulletPrefab;
    // location for where the bullet should spawn
    public Vector2 gunOffset = new Vector2(-1f, 0f);
    // gap between the 3 holes of the gun
    public float holeSpacing = 0.5f;
    // shortest time between shots
    public float minFireDelay = 1f;
    // longest time between shots
    public float maxFireDelay = 3f;
    // counts down to the next shot
    private float fireTimer;

    //components needed for the boss
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        //random wait before the first shot
        fireTimer = Random.Range(minFireDelay, maxFireDelay);
        ApplyGravityMode();
    }

    void Update()
    {
        //gravity switch
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            isFloating = !isFloating;
            ApplyGravityMode();
        }

        //shiver: move a little front and back (only when standing, not floating)
        if (!isFloating)
            rb.linearVelocity = new Vector2(Mathf.Sin(Time.time * shiverSpeed) * shiverAmount, rb.linearVelocity.y);

        //shooting count down, then fire and pick a new random wait
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Shoot();
            fireTimer = Random.Range(minFireDelay, maxFireDelay);
        }
    }

    // to create a new bullet at the gun tip (the bullet's own script makes it fly)
    void Shoot()
    {
        Vector2 gunTip = (Vector2)transform.position + gunOffset;
        // shoot 3 bullets at once: one from the top hole, middle hole and bottom hole
        Instantiate(bulletPrefab, gunTip + new Vector2(0, holeSpacing), bulletPrefab.transform.rotation);
        Instantiate(bulletPrefab, gunTip, bulletPrefab.transform.rotation);
        Instantiate(bulletPrefab, gunTip - new Vector2(0, holeSpacing), bulletPrefab.transform.rotation);
    }

    // to make the boss float or fall
    void ApplyGravityMode()
    {
        if (isFloating)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized * floatSpeed;
        }
        else
        {
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;   // X is not frozen so the boss can shiver
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    // to lower the boss health when it gets hit, no respawn so it is gone for good
    public void damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
