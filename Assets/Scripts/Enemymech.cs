using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Enemymech : MonoBehaviour
{
    //health
    public int maxHealth = 75;
    private int currentHealth;

    //player damage and range
    public int punchDamage = 25;
    public float punchRange = 1.5f;

    //random respawn for the mechs
    public float minRespawnDelay = 1f;
    public float maxRespawnDelay = 6f;
    public int maxLives = 5;
    private int deathcount = 0;

    //gravity mechanic
    public float floatSpeed = 1f;
    private bool isFloating = false;

    //shooting
    public GameObject bulletPrefab;                     // taking the bullet prefab and adding to the code
    public Vector2 gunOffset = new Vector2(-1f, 0f);    // location for where the bullet shuld spawn
    public float minFireDelay = 1f;                     // shortest time between shots
    public float maxFireDelay = 3f;                     // longest time between shots
    private float fireTimer;                            // counts down to the next shot

    //components needed for the mech
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private Transform player;
    private Vector3 spawnPosition;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        player = GameObject.FindWithTag("Player").transform;
        spawnPosition = transform.position;
        currentHealth = maxHealth;
        fireTimer = Random.Range(minFireDelay, maxFireDelay);   // random wait before the first shot
    }

    void Update()
    {
        //gravity switch
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            isFloating = !isFloating;
            if (!isDead) ApplyGravityMode();
        }

        //punch, to check if the player is alive, the player clicked the left mouse button and is close enough
        if (!isDead && Mouse.current.leftButton.wasPressedThisFrame &&
            Vector2.Distance(transform.position, player.position) <= punchRange)
        {
            TakeDamage(punchDamage);
        }

        //shooting count down, then fire and pick a new random wait
        if (!isDead)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                Shoot();
                fireTimer = Random.Range(minFireDelay, maxFireDelay);
            }
        }
    }

    // to create a new bullet at the gun tip (the bullet's own script makes it fly)
    void Shoot()
    {
        Vector2 gunTip = (Vector2)transform.position + gunOffset;
        Instantiate(bulletPrefab, gunTip, bulletPrefab.transform.rotation);
    }
    // to make the mech float or fall
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
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // to lower the mechs health when it gets hit
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        deathcount++;
        SetVisible(false);

        if (deathcount < maxLives) StartCoroutine(Respawn());
    }
    // respawn coroutine to bring the mech back to life
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(Random.Range(minRespawnDelay, maxRespawnDelay));

        transform.position = spawnPosition;
        currentHealth = maxHealth;
        SetVisible(true);
        isDead = false;
        ApplyGravityMode();
    }

    void SetVisible(bool visible)
    {
        sr.enabled = visible;
        col.enabled = visible;
        rb.simulated = visible;
    }
}