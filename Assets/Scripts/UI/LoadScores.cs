using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// script to manage leaderboard

public class LoadScores : MonoBehaviour
{
    public TMP_Text score1, score2, score3, player1, player2, player3;

    void Start()
    {
        LoadLeaderboard();
    }

    // to load leaderboard data
    void LoadLeaderboard()
    {
        // load leaderboard data from GameData
        GameData gameData = GameData.LoadData();

        // ensure the leaderboard is sorted by score (ascending order)
        gameData.leaderboard.Sort((a, b) => a.beatTime.CompareTo(b.beatTime));

        // display top 3 scores if available
        if (gameData.leaderboard.Count > 0)
        {
            player1.text = gameData.leaderboard[0].playerName;
            score1.text = gameData.leaderboard[0].beatTime.ToString() + " seconds";
        }
        else
        {
            player1.text = "---";
            score1.text = "---";
        }

        if (gameData.leaderboard.Count > 1)
        {
            player2.text = gameData.leaderboard[1].playerName;
            score2.text = gameData.leaderboard[1].beatTime.ToString() + " seconds";
        }
        else
        {
            player2.text = "---";
            score2.text = "---";
        }

        if (gameData.leaderboard.Count > 2)
        {
            player3.text = gameData.leaderboard[2].playerName;
            score3.text = gameData.leaderboard[2].beatTime.ToString() + " seconds";
        }
        else
        {
            player3.text = "---";
            score3.text = "---";
        }
    }

    // to clear all data from leaderboard
    public void ClearAllData()
    {
        // clear PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // clear in-memory leaderboard data by resetting the GameData instance
        GameData gameData = new GameData();
        gameData.SaveData();

        // clear the UI
        player1.text = "---";
        score1.text = "---";
        player2.text = "---";
        score2.text = "---";
        player3.text = "---";
        score3.text = "---";

        Debug.Log("All data cleared.");
    }

    // to return to the main menu
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
