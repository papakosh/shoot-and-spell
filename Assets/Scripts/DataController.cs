using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.Networking;

public class DataController : MonoBehaviour
{
    public LevelData[] allLevelData;
    private string gameDataFilename = "data.json";
    private string playerDataFilename = "player.json";
    public PlayerProgress playerProgress;
    private PlayerSettings playerSettings;
    private PlayerData playerData;

    public const int RECRUIT_RANK = 0;
    public const int CADET_RANK = 1;
    public const int PILOT_RANK = 2;
    public const int ACE_RANK = 3;
    public const int CHIEF_RANK = 4;
    public const int CAPTAIN_RANK = 5;
    public const int COMMANDER_RANK = 6;
    public const int MASTER_RANK = 7;

    public const string DIFFICULTY_EASY = "EASY";
    public const string DIFFICULTY_NORMAL = "NORMAL";
    public const string DIFFICULTY_HARD = "HARD";


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadGameData());
        StartCoroutine(LoadPlayerData());
        LoadPlayerProgress();
        LoadPlayerSettings();
        SceneManager.LoadScene("MainMenu");
    }

    private void LoadPlayerProgress()
    {
        playerProgress = new PlayerProgress(PlayerPrefs.GetInt("Rank"), PlayerPrefs.GetInt("XP"));
    }

    private void LoadPlayerSettings()
    {
        playerSettings = new PlayerSettings();
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            playerSettings.levelOfDifficulty = PlayerPrefs.GetString("Difficulty");
        }
        else
        {
            playerSettings.levelOfDifficulty = "EASY";
            PlayerPrefs.SetString("Difficulty", playerSettings.levelOfDifficulty);
        }
    }

    public void DisplayProgress(int level)
    {
        Debug.Log("words solved so far is on level " + (level + 1) + " is " + getCompletedLevelList(level).Count + " out of " + allLevelData[level].words.Length);
    }

    public List<String> getCompletedLevelList(int level)
    {
        switch (level + 1)
        {
            case 1:
                return playerData.level1Completed;
            case 2:
                return playerData.level2Completed;
            case 3:
                return playerData.level3Completed;
            case 4:
             return playerData.level4Completed;
            case 5:
               return playerData.level5Completed;
            case 6:
               return playerData.level6Completed;
            case 7:
             return playerData.level7Completed;
            case 8:
               return playerData.level8Completed;
            case 9:
               return playerData.level9Completed;
            case 10:
               return playerData.level10Completed;
            default:
                return null;
        }
    }

    public void MarkLevelComplete(int level)
    {
        allLevelData[level].isComplete = true;
        SaveGameData();

    }

    public void SavePlayerProgress(int rank, int xp, int level, string word )
    {
        playerProgress.rank = rank;
        playerProgress.xp = xp;
        PlayerPrefs.SetInt("Rank", rank);
        PlayerPrefs.SetInt("XP", xp);

        playerData.rank = rank;
        playerData.xp = xp;

        List<string> levelCompleted = getCompletedLevelList(level);
        if (!levelCompleted.Contains(word))
            levelCompleted.Add(word);

        SavePlayerData();
    }

    public int GetPlayerRank()
    {
        return playerData.rank;
    }

    public int GetPlayerXP()
    {
        return playerData.xp;
    }

    public void SavePlayerSettings (String difficulty)
    {
        PlayerPrefs.SetString("Difficulty", difficulty);
    }

    IEnumerator LoadGameData()
    {
        string filePath;
        filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);
        
        string dataAsJson;
        if (filePath.Contains("://") || filePath.Contains (":///")) 
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
            // deserialize string into object
            GameData loadData = JsonUtility.FromJson<GameData>(dataAsJson);
            allLevelData = loadData.allLevelData;
        }
        else if (File.Exists(filePath))
        {
            dataAsJson = File.ReadAllText(filePath);
            // deserialize string into object
            GameData loadData = JsonUtility.FromJson<GameData>(dataAsJson);
            allLevelData = loadData.allLevelData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    IEnumerator LoadPlayerData()
    {
        string filePath;
        filePath = Path.Combine(Application.streamingAssetsPath, playerDataFilename);

        string dataAsJson;
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
            // deserialize string into object
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
            //allLevelData = loadData.allLevelData;
            if (playerData != null && playerData.level10Completed != null)
            Debug.Log("level 1 data count is " + playerData.level10Completed.Count);
        }
        else if (File.Exists(filePath))
        {
            dataAsJson = File.ReadAllText(filePath);
            // deserialize string into object
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
            //allLevelData = loadData.allLevelData;
            if (playerData != null && playerData.level1Completed != null)
                Debug.Log("level 1 data count is " + playerData.level1Completed.Count);
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }

    }

    private void SaveGameData()
    {
        GameData gameData = new GameData();
        gameData.allLevelData = allLevelData;
        string dataAsJson = JsonUtility.ToJson(gameData);
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);
        File.WriteAllText(filePath, dataAsJson);
    }

    void SavePlayerData()
    {
        string dataAsJson = JsonUtility.ToJson(playerData);
        string filePath = Path.Combine(Application.streamingAssetsPath, playerDataFilename);

        /*if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            www.downloadHandler.text = dataAsJson;
            // deserialize string into object
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
            //allLevelData = loadData.allLevelData;
            if (playerData != null && playerData.level10Completed != null)
                Debug.Log("level 1 data count is " + playerData.level10Completed.Count);
        }
        else *///if (File.Exists(filePath))
       // {
            //dataAsJson = File.ReadAllText(filePath);
            File.WriteAllText(filePath, dataAsJson);
            // deserialize string into object
            //playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
            //allLevelData = loadData.allLevelData;
        //}
        //else
       // {
        //    Debug.LogError("Cannot load game data!");
        //}

    }
}
