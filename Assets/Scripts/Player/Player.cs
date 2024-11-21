using UnityEngine;
using System.Collections;
using TMPro;

public class Player : MonoBehaviour
{
    // random variables
    public Manager manager;

    // health variables
    public int lives = 3;
    public int maxLives = 3;
    public int minLives = 0;
    public float invincibilityDuration = 1f;
    public delegate void HealthChanged(int newHealth);
    public event HealthChanged OnHealthChanged;

    // UI / music variables
    public TextMeshProUGUI healthRegenText;
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip healSound; 
    private SpriteRenderer spriteRenderer;

    // boolean variables
    public bool dead = false;
    private bool isInvincible = false;

    // animation variables
    Animator m_Animator;

    public void Start() {
        m_Animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update() {
        if (lives == 0) {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if player has reached the end of the level (win)
        if (other.CompareTag("WinTrigger"))
        {
            manager.Win();
        }

        // check if the player collides with health regen item
        if (other.CompareTag("HealthRegenTrigger")) {
            lives = maxLives;
            audioSource.PlayOneShot(healSound);
            healthRegenText.text = "Health Regenerated!";
            OnHealthChanged?.Invoke(lives);
            StartCoroutine(DelayHealthRegenText());
            Destroy(other.gameObject);
        }

        // check if player collides with spike
        if (other.CompareTag("Spike")) {
            GetHit();
        }
    }

    // method to handle when player is hit or takes damage
    public void GetHit()
    {
        if (isInvincible) return;  // don't process if the player is invincible

        audioSource.PlayOneShot(hurtSound);
        Debug.Log("Player has been hit.");
        m_Animator.SetTrigger("IsHitTrigger");

        lives -= 1;

        lives = Mathf.Clamp(lives, 0, maxLives);

        OnHealthChanged?.Invoke(lives);

        // check if player has died
        if (lives == 0)
        {
            m_Animator.SetTrigger("IsDeadTrigger");
        }
        else
        {
            StartCoroutine(InvincibilityCooldown());
        }
    }

    // method to handle player death
    private void Die()
    {
        // wait for the death animation before triggering the game over screen
        m_Animator.SetTrigger("IsDeadTrigger");
        StartCoroutine(DelayDeath());  // start the delay before transitioning to the game over screen
    }

    // method to delay death animation
    IEnumerator DelayDeath() {
        yield return new WaitForSeconds(1f); // wait for 1 second for the death animation
        m_Animator.speed = 0;  // pause the animation
        dead = true;
    }

    // method to delay health regen text
    IEnumerator DelayHealthRegenText() {
        yield return new WaitForSeconds(3f);
        healthRegenText.text = "";
    }


    // invincibility cooldown with blinking effect
    private IEnumerator InvincibilityCooldown()
    {
        isInvincible = true;
        float timePassed = 0f;

        // start invincibility with blinking effect
        while (timePassed < invincibilityDuration)
        {
            // toggle visibility of sprite renderer on and off to create blinking effect
            spriteRenderer.enabled = !spriteRenderer.enabled;
            timePassed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
