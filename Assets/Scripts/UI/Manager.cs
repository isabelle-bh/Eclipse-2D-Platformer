using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// manager script for level 0 scene

public class Manager : MonoBehaviour
{
    // boolean variables
    private bool isGameOver = false;
    public bool isPaused = false;

    // game data variables
    public float timer = 0f;
    public int enemiesKilled = 0;

    // UI variables
    public Player playerHealth;
    public TextMeshProUGUI livesText;
    public Player player;

    void Start()
    {
        // initialize the lives text
        UpdateLivesText();
        // reset timer when level started / restarted
        timer = 0f;
    }

    void Update()
    {
        // update timer if game not over
        if (!isGameOver)
        {
            timer += Time.deltaTime;
        }

        // check if player is dead and game over if so
        if (playerHealth.dead == true) {
            GameOver();
        }

        // update the lives display
        UpdateLivesText();
    }

    // method to dynamically update the lives text on screen
    void UpdateLivesText()
    {
        livesText.text = "Lives: " + playerHealth.lives;
    }

    // method to call when the player loses all lives
    void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            timer = Mathf.Round(timer * 100f) / 100f;
            SceneManager.LoadScene("GameOver");
        }
    }

    // method to call when the player wins the level
    public void Win() {
        GameData.enemiesKilled = enemiesKilled;
        GameData.timer = timer;
        SceneManager.LoadScene("Win");
    }
}
