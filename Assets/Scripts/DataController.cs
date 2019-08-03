using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Text;

public class DataController : MonoBehaviour
{
    public LevelData[] allLevelData;
    private string gameDataFilename = "data.json";
    private string playerDataFilename = "player.json";
    public PlayerData playerData;
    public LevelOfDifficulty currentDifficulty;

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
        LoadPlayerData();
        currentDifficulty = GetCurrentDifficulty();
        // show splash screen
        StartCoroutine(LoadTheGame());
    }

    private IEnumerator LoadTheGame()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("MainMenu");
        yield return new WaitForSeconds(2.0f);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public List<String> getCompletedLevelList(int level)
    {
        switch (level+1)
        {
            case 1:
                return currentDifficulty.level1Completed;
            case 2:
                return currentDifficulty.level2Completed;
            case 3:
                return currentDifficulty.level3Completed;
            case 4:
             return currentDifficulty.level4Completed;
            case 5:
               return currentDifficulty.level5Completed;
            case 6:
               return currentDifficulty.level6Completed;
            case 7:
             return currentDifficulty.level7Completed;
            case 8:
               return currentDifficulty.level8Completed;
            case 9:
               return currentDifficulty.level9Completed;
            case 10:
               return currentDifficulty.level10Completed;
            default:
                return null;
        }
    }
    public LevelOfDifficulty GetCurrentDifficulty()
    {
        switch (playerData.difficultySelected)
        {
            case "EASY":
                return playerData.easyDifficulty;
            case "NORMAL":
                return playerData.normalDifficulty;
            case "HARD":
                return playerData.hardDifficulty;
            default:
                return playerData.easyDifficulty;
        }
    }

    public void UnlockNextLevel (int level)
    {
        currentDifficulty.levelsUnlocked[level+1] = true;
        SavePlayerData();
    }

    public void UnlockNormalAndHardDifficulty()
    {
        playerData.difficultyUnlocked[1] = true;
        playerData.difficultyUnlocked[2] = true;
        SavePlayerData();
    }

    public void SavePlayerProgress(int rank, int xp, int level, string word )
    {
        currentDifficulty.rank = rank;
        currentDifficulty.xp = xp;

        List<string> levelCompleted = getCompletedLevelList(level);
        if (!levelCompleted.Contains(word))
            levelCompleted.Add(word);

        SavePlayerData();
    }

    public int GetPlayerRank()
    {
        return currentDifficulty.rank;
    }

    public int GetPlayerXP()
    {
        return currentDifficulty.xp;
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(2.0f);
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

    private void LoadPlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFilename);
        if (File.Exists(filePath))
        {
            byte[] jsonBytes = File.ReadAllBytes(filePath);
            string dataAsJson = Encoding.ASCII.GetString(jsonBytes);
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
        }
        else
        {
            playerData = new PlayerData();
            string dataAsJson = JsonUtility.ToJson(playerData);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(dataAsJson);
            File.WriteAllBytes(filePath, jsonBytes);

            byte[] jsonBytesRead = File.ReadAllBytes(filePath);
            string dataAsJsonRead = Encoding.ASCII.GetString(jsonBytesRead);
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJsonRead);
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
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFilename);
        byte[] bytes = Encoding.ASCII.GetBytes(dataAsJson);
        File.WriteAllBytes(filePath, bytes);
    }

    public void UpdatePlayerDifficulty(String difficultySelected)
    {
        if (playerData.difficultySelected.Equals(difficultySelected))
            return;
        playerData.difficultySelected = difficultySelected;
        SavePlayerData();
        currentDifficulty = GetCurrentDifficulty();
    }
}
