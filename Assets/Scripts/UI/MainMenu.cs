using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// script to manage main menu scene

public class MainMenu : MonoBehaviour
{
    // audio variables
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    // to start the game
    public void StartGame()
    {
        StartCoroutine(PlaySoundAndLoadScene("Instructions"));
    }

    // to load leaderboard
    public void Leaderboard()
    {
        SceneManager.LoadScene("LoadScores"); // Load the new scene
    }

    // to quit game
    public void QuitGame()
    {
        Application.Quit(); // Close the game
    }

    // method to play click sound and then load scene
    private IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        audioSource.PlayOneShot(buttonClickSound);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
