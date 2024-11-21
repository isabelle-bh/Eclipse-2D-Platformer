using UnityEngine;

public class CameraController : MonoBehaviour
{
    // reference to player object
    [SerializeField] private Transform player;

    private void Update()
    {
        // follow player on x and y axis
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }
}
