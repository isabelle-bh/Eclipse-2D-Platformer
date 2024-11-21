using UnityEngine;
using UnityEngine.SceneManagement;

// script to allow music to play throughout different scenes (eg. main menu to leaderboard, main menu to instructions)

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // ensure only one instance of MusicManager exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // destroy duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // to keep this object across scenes
    }

    // destroy music manager in gameplay scene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level0")
        {
            Destroy(gameObject); // Destroy music manager in gameplay scene
        }
    }

    // subscribe to scene loaded event
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // unsubscribe from scene loaded event
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}