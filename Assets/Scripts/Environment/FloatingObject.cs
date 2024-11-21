using UnityEngine;

// script to make an object float up and down

public class FloatingObject : MonoBehaviour
{
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 1.0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // calculate the new y position using a sine wave function
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        // apply the new position while keeping the x and z positions the same
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
