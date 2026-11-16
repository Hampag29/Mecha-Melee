using UnityEngine;
using UnityEngine.InputSystem;

public class Medkit : MonoBehaviour
{
    public float beltSpeed = 2f;
    public float fallSpeed = 1f;
    public float slowFallSpeed = 0.5f;
    Rigidbody2D rb;
    Vector3 startPos;
    bool falling;
    bool lowGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        Hide();
    }

    void Update()
    {
        // Q flips gravity mode
        if (Keyboard.current.qKey.wasPressedThisFrame) lowGravity = !lowGravity;  
        // using ? and : to create a condition, I had to learn this seperatly to make the code shorter
        if (falling) rb.linearVelocity = Vector2.down * (lowGravity ? slowFallSpeed : fallSpeed);
        // to ride the belt at beltspeed if it does not fall
        else transform.position += Vector3.left * beltSpeed * Time.deltaTime;      
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        // if the medkit touches the player, the player gains 35 health
        PlayerController player = hit.gameObject.GetComponent<PlayerController>();
        if (player != null) player.setHealth(35);

        Enemymech mech = hit.gameObject.GetComponent<Enemymech>();
        if (mech != null) mech.Heal(35);
        if (mech != null || player != null) Hide();
    }

    void Hide()   
    {
        //teleport meddkit out of the screen view 
        transform.position = new Vector3(100f, 100f, 0f);
        rb.linearVelocity = Vector2.zero;
        falling = false;
        //using invoke to run back the backtobelt function, had to learn this seperatly 
        Invoke("BackToBelt", Random.Range(3f, 8f));
    }

    // to reappear on the belt, drop at a random time
    void BackToBelt()   
    {
        transform.position = startPos;
        Invoke("Drop", Random.Range(1f, 8f));
    }

    void Drop() { falling = true; }
}