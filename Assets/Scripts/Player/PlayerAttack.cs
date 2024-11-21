using System.Collections;
using UnityEngine;

// script to handle player attack behaviour

public class PlayerAttack : MonoBehaviour
{
    // random variables
    public PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    // attack variables
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] public int attackDamage = 10;
    private RaycastHit2D[] hits;

    // animator variables
    public Animator m_Animator;
    [SerializeField] private Transform spikePoint;
    [SerializeField]private GameObject[] spikeballs;

    // enemy variables
    [SerializeField] private LayerMask enemyLayer;

    // audio variables
    public AudioSource audioSource;
    public AudioClip meleeSound;
    public AudioClip rangedSound;

    // UI variables
    public PauseMenu pauseMenu;

    // boolean variables
    public bool isAttacking;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!pauseMenu.gameIsPaused) {
            // check if player is grounded and if the player has pressed the attack button
            if (Input.GetButtonDown("Fire1") && !isAttacking && playerMovement.isGrounded) {
                Attack();
                audioSource.PlayOneShot(meleeSound);
            }
            // check if player has pressed the ranged attack button and if the cooldown timer has passed the attack cooldown
            if (Input.GetButtonDown("Fire2") && cooldownTimer > attackCooldown && playerMovement.canAttack()) {
                RangedAttack();
                audioSource.PlayOneShot(rangedSound);
            }
            cooldownTimer += Time.deltaTime;    
        }
    }

    // method to handle the ranged attack
    private void RangedAttack()
    {
        cooldownTimer = 0;
        spikeballs[0].transform.position = spikePoint.position;
        spikeballs[0].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    // method to handle the melee attack
    void Attack() {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, enemyLayer);
        isAttacking = true;
        Debug.Log("Player attacking");

        m_Animator.SetTrigger("AttackTrigger");

        for (int i = 0; i < hits.Length; i++) {
            Enemy enemyComponent = hits[i].collider.gameObject.GetComponent<Enemy>();
            if (enemyComponent != null) {
                enemyComponent.TakeDamage(attackDamage);
            }
        }
        StartCoroutine(AttackCooldown());
    }

    // method to handle the attack cooldown
    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
    
    // method to handle the end of the attack animation
    public void FinishAttack()
    {
        isAttacking = false;
    }   

    // draw the attack range in the editor
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}
