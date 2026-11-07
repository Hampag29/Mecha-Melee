using UnityEngine;

public class Bulletcontroller : MonoBehaviour
{

    //intializing variables 

    public float speed = 8f;                  
    public int damage = 15;                   
    public float lifetime = 3f;               
    public Vector2 direction = Vector2.left;  
    private Rigidbody2D rb;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) return; 
        if (other.isTrigger) return;          


        Destroy(gameObject);  
    }
}
