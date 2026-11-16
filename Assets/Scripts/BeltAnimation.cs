using UnityEngine;

public class BeltAnimation : MonoBehaviour
{

    // creating list of pictures to play,
    public Sprite[] frames;
    // seconds each picture shows
    public float frameTime = 0.1f; 

    private SpriteRenderer sr;
    private float timer;
    private int currentFrame = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // delta time is used to count the time since the last frame
        timer += Time.deltaTime;    
        //if condition to check if the timer has passed 0.1 seconds, if it did then it needs to go on to the next frame
        if (timer >= frameTime)              
        {
            timer = 0;
            //go to the next frame
            currentFrame++;                  
            if (currentFrame >= frames.Length)
                //if the final frame has been run, start from the first frame agaibn
                currentFrame = 0;            

            //to show the next frame
            sr.sprite = frames[currentFrame];  
        }
    }
}
