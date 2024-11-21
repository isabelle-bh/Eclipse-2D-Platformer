using System.Collections;
using UnityEngine;

// script to control all skeleton enemy behaviour

public class Enemy : MonoBehaviour
{
    // random
    private Rigidbody2D body;
    public LayerMask playerLayer;
    public Manager manager;
    private Transform playerTransform;

    // audio variables
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip hurtSound;

    // Movement variables
    [SerializeField] private float movementSpeed = 1.5f;
    [SerializeField] private float minChangeDirectionTime = 3f;
    [SerializeField] private float maxChangeDirectionTime = 8f;
    [SerializeField] private float jumpForce = 5f; // New jump force variable
    private Vector2 movementDirection;
    private float directionChangeTimer;

    // attack and health variables
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    public Transform enemyAttackBox;
    [SerializeField] private float maxHealth = 30f;
    private float currentHealth;
    public float deathDelay = 1.5f;


    // booleans
    public bool isDead = false;
    private bool isEdgeAhead = false;
    public bool isHit;
    public bool isWalking;
    private bool isGrounded = false;
    private bool isFollowingPlayer = false;
    public bool isAttacking = false;

    // animation variables
    public Animator m_Animator;

    // edge check variables
    public Transform edgeCheck;  // Drag the EdgeCheck object here in the Inspector

    // wall check variables
    public float wallCheckDistance = 0.5f;
    public Transform wallCheck;

    // ground check variables
    public Transform groundCheck;  // Drag the GroundCheck object here in the Inspector
    public LayerMask groundLayer;  // Assign the Ground Layer here in the Inspector
    private float groundCheckDistance = 0.1f; // Distance for raycast to detect ground

    void Start() {
        body = GetComponent<Rigidbody2D>();
        movementDirection = Vector2.right; // Initially moving right
        StartCoroutine(ChangeDirection());
        m_Animator = GetComponent<Animator>();
        manager = GameObject.Find("Manager").GetComponent<Manager>();

        currentHealth = maxHealth;

        // checking if player object in scene
        GameObject player = GameObject.Find("player");
        if (player != null) {
            playerTransform = player.transform;
        }
    }

    void Update() {
        // check for wall and make the enemy jump
        if (IsWallAhead() && isGrounded) {
            Jump();
        }

        // looking for player
        if (playerTransform != null) {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= detectionRange) {
                isFollowingPlayer = true;
                if (distanceToPlayer <= attackRange && !isAttacking) {
                    // attack the player if in range
                    AttackPlayer();
                }
            }
            else {
                isFollowingPlayer = false;
            }
        }
    }

    // method to attack the player
    void AttackPlayer() {
        if (isAttacking) return;

        Debug.Log("Enemy attacking player");
        isAttacking = true;
        m_Animator.SetBool("IsLocked", true);

        m_Animator.SetTrigger("AttackTrigger");
        body.velocity = Vector2.zero;

        StartCoroutine(DelayedDealDamage());
        StartCoroutine(AttackCooldown());
    }


    // method for attack cooldown
    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        m_Animator.SetBool("IsLocked", false); // Unlock the animation
    }


    // method to delay damage window so that it hits halfway through the attack animation
    IEnumerator DelayedDealDamage() {
        // wait for half the attack animation duration (adjust the timing as necessary)
        yield return new WaitForSeconds(1f);

        // check if the player is within the enemy's attack box
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(
            enemyAttackBox.position, 
            enemyAttackBox.localScale, 
            0f, 
            playerLayer
        );

        // damage the player if hit
        foreach (Collider2D hit in hitPlayers) {
            Player player = hit.GetComponent<Player>();
            if (player != null) {
                player.GetHit(); // Damage the player
                Debug.Log("Player hit by enemy attack!");
            }
        }
    }

    void FixedUpdate() {
        isWalking = movementDirection != Vector2.zero;

        // update animator parameter to trigger walk animation
        m_Animator.SetBool("IsWalking", isWalking);

        // update movement velocity
        body.velocity = new Vector2(movementDirection.x * movementSpeed, body.velocity.y);
        
        // check if grounded
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // move the enemy towards the player if in range
        if (isFollowingPlayer && !isAttacking) {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            body.velocity = new Vector2(direction.x * movementSpeed, body.velocity.y);
            // flip the sprite based on the direction
            if (direction.x != 0) {
                transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        // check if there's ground ahead in the direction of movement
        isEdgeAhead = Physics2D.Raycast(edgeCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // reverse direction if no ground is detected ahead
        if (!isEdgeAhead && isGrounded) {
            movementDirection = -movementDirection;
            Flip((int)movementDirection.x); // Flip the sprite
        }
    }

    // method to change direction of movement
    private IEnumerator ChangeDirection() {
        while(true) {
            directionChangeTimer = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
            yield return new WaitForSeconds(directionChangeTimer);

            movementDirection = -movementDirection; // Change direction
            Flip((int)movementDirection.x); // Flip the sprite
        }
    }

    // method to check if wall is ahead
    private bool IsWallAhead() {
        // Perform a raycast in the direction the enemy is moving
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, movementDirection, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    // method to jump
    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, jumpForce); // Set vertical velocity for a jump
    }

    // method to flip the sprite based on direction
    private void Flip(int newDirection) {
        // Flip the enemy sprite based on the direction
        float scaleX = Mathf.Abs(transform.localScale.x) * newDirection;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage(int damage) {
        isHit = true;
        audioSource.PlayOneShot(hurtSound);
        m_Animator.SetTrigger("IsHitTrigger");

        maxHealth -= damage;
        if (maxHealth <= 0 && !isDead) {
            Die();
        }
    }

    // method to finish hit animation
    public void FinishHit() {
        isHit = false;
    }

    // method for when enemy dies
    void Die() {
        isDead = true;
        audioSource.PlayOneShot(deathSound);
        m_Animator.SetBool("IsHealth0", true);

        Destroy(gameObject, deathDelay);
        manager.enemiesKilled += 1;
    }

    // to visualize checks and boxes in the scene editor
    private void OnDrawGizmos() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
        if (wallCheck != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
        }
        if (edgeCheck != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * groundCheckDistance);
        }
        if (enemyAttackBox != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(enemyAttackBox.position, enemyAttackBox.localScale);
        }
    }
}
