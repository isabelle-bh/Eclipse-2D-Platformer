using UnityEngine;

// script to control the flying eye enemy behaviour

public class FlyingEye : MonoBehaviour
{
    // movement variables
    public float moveSpeed = 2f;
    public float patrolTime = 3f;
    private float patrolTimer;
    private bool movingRight = true;

    void Start()
    {
        patrolTimer = patrolTime;
    }

    void Update()
    {
        // move the Flying Eye in the current direction
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        // decrement the patrol timer as time passes
        patrolTimer -= Time.deltaTime;

        // when the patrol timer runs out, reverse direction and reset the timer
        if (patrolTimer <= 0)
        {
            movingRight = !movingRight; // Switch direction
            Flip(movingRight ? 1 : -1); // Flip the sprite based on direction
            patrolTimer = patrolTime; // Reset the patrol timer
        }
    }

    // flip the enemy sprite based on the direction
    private void Flip(int newDirection)
    {
        float scaleX = Mathf.Abs(transform.localScale.x) * newDirection;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    // collision detection with the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if the player collides with the enemy and hit player if so
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.GetHit();
            }
        }
    }
}
