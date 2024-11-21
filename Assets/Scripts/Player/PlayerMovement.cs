using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // movement variables
    private Rigidbody2D body;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 jumpHeight;
    private float timeFalling = 0f;
    private Vector3 spawnPosition;

    // audio variables
    public AudioSource audioSource;
    public AudioClip runSound;

    // animator variables
    Animator m_Animator;

    // health variables
    private Player playerHealth;

    // boolean variables
    public bool isGrounded = false;

    // ground check variables
    public Transform groundCheck;
    public Transform groundCheckLeft;
    public Transform groundCheckRight;

    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<Player>();
    }

    void Start() {
        m_Animator = GetComponent<Animator>();
        spawnPosition = transform.position;
    }

    private void Update() {
        // check if the player is grounded using raycast
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) ||
             Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, groundLayer) ||
             Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, groundLayer);

        // run movement
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        // jump movement
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            body.AddForce(jumpHeight, ForceMode2D.Impulse);
        }

        if (isGrounded) {
            m_Animator.SetBool("IsJumping", false);
            m_Animator.SetBool("IsFalling", false);
        }
    }

    void FixedUpdate() {

        // check if player is running
        float horizontal = Input.GetAxis("Horizontal");
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool isRunning = hasHorizontalInput;
        m_Animator.SetBool("IsRunning", isRunning);

        // play running sound
        if (isRunning && isGrounded) {
            if (!audioSource.isPlaying)  // prevent overlapping sounds
            {
                audioSource.PlayOneShot(runSound);
            }
        }

        // check if player is falling
        bool isFalling = !isGrounded && body.velocity.y < 0;
        m_Animator.SetBool("IsFalling", isFalling);

        // check if player is jumping
        bool isJumping = !isGrounded && body.velocity.y > 0;
        m_Animator.SetBool("IsJumping", isJumping);

        // check if the player is falling
        if (isFalling)
        {
            timeFalling += Time.deltaTime;

            // if the player has fallen for more than 5 seconds, respawn them
            if (timeFalling > 3f && playerHealth.lives > 0)
            {
                playerHealth.lives -= 1;
                if (playerHealth.lives > 0)
                {
                    Respawn();
                }
            }
        } else {
            timeFalling = 0f;
        }

        // flip the sprite based on the direction
        if (horizontal > 0.01f) {
            transform.localScale = new Vector3(5.5f, 6, 1);
        } else if (horizontal < -0.01f) {
            transform.localScale = new Vector3(-5.5f, 6, 1);
        }
    }

    // method to respawn the player
    private void Respawn() {
        transform.position = new Vector3(-4,2,0);
        timeFalling = 0f;
    }

    // method to check if player can attack
    public bool canAttack() {
        return isGrounded;
    }

    // draw a line to show the ground check radius
    private void OnDrawGizmos() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);
        }
        if (groundCheckLeft != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckLeft.position, groundCheckLeft.position + Vector3.down * groundCheckRadius);
        }        
        if (groundCheckRight != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckRight.position, groundCheckRight.position + Vector3.down * groundCheckRadius);
        }
    }
}
