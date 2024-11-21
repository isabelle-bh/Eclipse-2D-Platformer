using UnityEngine;
using UnityEngine.SceneManagement;

// script to manage instructions scene

public class Instructions : MonoBehaviour
{
    void FixedUpdate()
    {
        // if any key is pressed, load the first level
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Level0");
        }
    }
}
