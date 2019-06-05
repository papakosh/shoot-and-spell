using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class DataController : MonoBehaviour
{
    public LevelData[] allLevelData;
    private string gameDataFilename = "data.json";
    private PlayerProgress playerProgress;
    private PlayerSettings playerSettings;
    
    public const int PRESCHOOL_RANK = 0;
    public const int KINDERGARTEN_RANK = 1;
    public const int FIRSTGRADE_RANK = 2;
    public const int SECONDGRADE_RANK = 3;
    public const int THIRDGRADE_RANK = 4;
    public const int FOURTHGRADE_RANK = 5;
    public const int FIFTHGRADE_RANK = 6;
    public const int SIXTHGRADE_RANK = 7;
    public const int SEVENTHGRADE_RANK = 8;
    public const int EIGHTHGRADE_RANK = 9;
    public const int NINTHGRADE_RANK = 10;
    public const int TENTHGRADE_RANK = 11;
    public const int ELEVENTHRADE_RANK = 12;
    public const int TWELFTHGRADE_RANK = 13;

    public const string DIFFICULTY_EASY = "EASY";
    public const string DIFFICULTY_NORMAL = "NORMAL";
    public const string DIFFICULTY_HARD = "HARD";


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadGameData();
        LoadPlayerProgress();
        LoadPlayerSettings();
        SceneManager.LoadScene("MainMenu");
    }

    private void LoadPlayerProgress()
    {
        playerProgress = new PlayerProgress();
        playerProgress.rank = PlayerPrefs.GetInt("Rank");
        playerProgress.xp = PlayerPrefs.GetInt("XP");
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

    public void SavePlayerProgress(int rank, int xp )
    {
        playerProgress.rank = rank;
        playerProgress.xp = xp;
        PlayerPrefs.SetInt("Rank", rank);
        PlayerPrefs.SetInt("XP", xp);
    }

    public void SavePlayerSettings (String difficulty)
    {
        PlayerPrefs.SetString("Difficulty", difficulty);
    }

    private void LoadGameData()
    {
        string filePath;
#if UNITY_EDITOR
        filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);
#elif UNITY_ANDROID
        filePath = "jar:file://" + Application.dataPath + "!/assets/" + gameDataFilename;
#endif
        if (File.Exists(filePath))
        {
            // read all text into string
            string dataAsJson = File.ReadAllText(filePath);

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
        if (PlayerPrefs.GetInt("FirstTime") == 0) // First time, load defaults
        {

        }
    }
}
