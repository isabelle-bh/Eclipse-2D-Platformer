using System;
using UnityEngine;

// script to control projectile behaviour

public class Projectile : MonoBehaviour
{
    // movement variables
    [SerializeField] private float speed = 5f;
    private float direction;

    // damage variables
    [SerializeField] private int damage = 1;
    private bool hit;
    private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D is missing!");
        }
    }

    void Update()
    {
        // skip movement if the projectile has hit something
        if (hit) return;

        // move the projectile
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }

    // check if the projectile hits an enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Deactivate(); // deactivate the projectile after hitting the enemy
        }
    }

    // set the direction of the projectile
    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // flip the projectile to match direction
        Vector3 localScale = transform.localScale;
        if (Math.Sign(localScale.x) != _direction)
        {
            localScale.x = -localScale.x;
        }
        transform.localScale = localScale;
    }

    // method to deactivate the projectile after hitting the enemy
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
