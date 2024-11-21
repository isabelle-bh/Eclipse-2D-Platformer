using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used to store game data such as player name and time beat
// mainly used for leaderboard

[System.Serializable]
public class GameData
{
    public static int enemiesKilled;
    public static float timer;

    [System.Serializable]
    public class PlayerRecord {
        public string playerName;
        public float beatTime;
    }

    public List<PlayerRecord> leaderboard = new List<PlayerRecord>();

    // add player record to leaderboard
    public void AddPlayerRecord(string name, float time) {
        leaderboard.Add(new PlayerRecord { playerName = name, beatTime = time });
        SaveData();
    }

    // save data to file
    public void SaveData() {
        string json = JsonUtility.ToJson(this, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/GameData.json", json);
    }

    // load data from file
    public static GameData LoadData()
    {
        string filePath = Application.persistentDataPath + "/GameData.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return new GameData(); // Return a new GameData if no file exists
    }
}
