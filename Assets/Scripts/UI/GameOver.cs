using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// script to manage gameover scene

public class GameOver : MonoBehaviour
{
    public void Start() {
        StartCoroutine(DelayedButtonAppearance());
    }
    
    // method to restart the game
    public void Restart() {
        SceneManager.LoadScene("Level0");
    }

    // method to go back to main menu
    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    // to allow video to play a bit before buttons appear
    IEnumerator DelayedButtonAppearance() {
        yield return new WaitForSeconds(5f);

        // find all button components in the scene
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in buttons) {
            if (button.CompareTag("UI")) {
                button.gameObject.SetActive(true);
            }
        }
    }
}