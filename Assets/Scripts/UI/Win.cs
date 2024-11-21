using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// script to manage win scene

public class Win : MonoBehaviour
{
    // UI variables
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI timeToBeatText;
    public TextMeshProUGUI playstyleText;
    public TMP_InputField playerNameInput;
    public Button submitButton;

    public void Start()
    {
        // disable the submit button initially
        submitButton.interactable = false;

        // add a listener to the input field to enable/disable the submit button based on the player name length
        playerNameInput.onValueChanged.AddListener(ValidatePlayerName);

        UpdateMenuText();
    }

    // called when the input field value changes
    void ValidatePlayerName(string playerName)
    {
        // enable submit button if the player name is at least 3 characters long
        if (playerName.Length >= 3)
        {
            submitButton.interactable = true;
        }
        else
        {
            submitButton.interactable = false;
        }
    }

    // method to update the menu text based on game data
    void UpdateMenuText()
    {
        // display the number of enemies killed and time to beat the level
        enemiesKilledText.text = "you killed this many enemies: " + GameData.enemiesKilled;
        timeToBeatText.text = "time to beat level 0: " + Math.Round(GameData.timer, 2) + " seconds";

        // determine the playstyle based on the number of enemies killed and time taken
        if (GameData.enemiesKilled >= 7)
        {
            playstyleText.text = "playstyle: killer.";
        }
        else if (GameData.enemiesKilled > 0 && GameData.timer < 120)
        {
            playstyleText.text = "playstyle: casual.";
        }
        else if (GameData.enemiesKilled == 0 && GameData.timer < 90)
        {
            playstyleText.text = "playstyle: speedrunner... or coward?";
        }
        else if (GameData.timer >= 120)
        {
            playstyleText.text = "playstyle: turtle!";
        }
        else
        {
            playstyleText.text = "playstyle: average.";
        }
    }

    // method to submit the player's score and name
    public void SubmitScore()
    {
        // get player name from the input field
        string playerName = playerNameInput.text.Trim();

        // validate input
        if (string.IsNullOrEmpty(playerName) || playerName.Length < 3)
        {
            Debug.LogWarning("Player name is too short. Please enter at least 3 characters.");
            return;
        }

        // calculate score by simply rounding timer to 2 decimal places
        float score = (float)Math.Round(GameData.timer, 2);

        // submit to leaderboard
        SubmitToLeaderboard(playerName, score);

        // feedback for debugging purposes
        Debug.Log($"Score submitted! Player: {playerName}, Score: {score}");
    }

    // method to take submitted score and name and put onto leaderboard
    void SubmitToLeaderboard(string playerName, float score)
    {
        // load data
        GameData gameData = GameData.LoadData();
        // add player record to leaderboard
        gameData.AddPlayerRecord(playerName, score);
        // save data
        gameData.SaveData();

        // load main menu after submitting score
        Debug.Log($"Player {playerName} with score {score} added to leaderboard.");
        SceneManager.LoadScene("MainMenu");
    }
}