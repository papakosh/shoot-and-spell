using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Text;

public class DataController : MonoBehaviour
{
    public GameData gameData;
    public PlayerData playerData;
    public Difficulty currentDifficulty;

    // Rank Constants
    public const int RECRUIT_RANK = 0;
    public const int CADET_RANK = 1;
    public const int PILOT_RANK = 2;
    public const int ACE_RANK = 3;
    public const int CHIEF_RANK = 4;
    public const int CAPTAIN_RANK = 5;
    public const int COMMANDER_RANK = 6;
    public const int MASTER_RANK = 7;

    // Difficulty Constants
    public const string DIFFICULTY_EASY = "EASY";
    public const string DIFFICULTY_NORMAL = "NORMAL";
    public const string DIFFICULTY_HARD = "HARD";

    // Setting Constants
    public const string MUSIC_VOLUME = "MUSIC_VOL";
    public const string WEAPONS_VOLUME = "WEAPONS_VOL";
    public const string EXPLOSIONS_VOLUME = "EXPLOSIONS_VOL";
    public const string WORDS_VOLUME = "WORDS_VOL";
    public const string PICKUPS_VOLUME = "PICKUPS_VOL";
    public const string JOYSTICK_CONTROL = "JOYSTICK_CONTROL";
    public const string JOYSTICK_CONTROL_LEFT = "LEFT-HANDED";
    public const string JOYSTICK_CONTROL_RIGHT = "RIGHT-HANDED";
    public const float DEFAULT_VOL = 0.5f;

    // Game files
    private string gameDataFilename = "data.json";
    private string playerDataFilename = "player.json";

    // Volume defaults
    private float musicVolDefault = 0.05f;
    private float weaponsVolDefault = 0.2f;
    private float explosionsVolDefault = 0.5f;
    private float wordsVolDefault = 1.0f;
    private float pickupsVolDefault = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadGameData());
        LoadPlayerData();
        currentDifficulty = GetCurrentDifficulty();
        LoadSettings();
        StartCoroutine(LoadMainMenu());
    }

    public List<String> getCompletedLevelList(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0:
                return currentDifficulty.level1CompletedWords;
            case 1:
                return currentDifficulty.level2CompletedWords;
            case 2:
                return currentDifficulty.level3CompletedWords;
            case 3:
                return currentDifficulty.level4CompletedWords;
            case 4:
                return currentDifficulty.level5CompletedWords;
            case 5:
                return currentDifficulty.level6CompletedWords;
            case 6:
                return currentDifficulty.level7CompletedWords;
            case 7:
                return currentDifficulty.level8CompletedWords;
            case 8:
                return currentDifficulty.level9CompletedWords;
            case 9:
                return currentDifficulty.level10CompletedWords;
            default:
                return null;
        }
    }
    public Difficulty GetCurrentDifficulty()
    {
        switch (playerData.difficultySelected)
        {
            case DIFFICULTY_EASY:
                return playerData.easyDifficulty;
            case DIFFICULTY_NORMAL:
                return playerData.normalDifficulty;
            case DIFFICULTY_HARD:
                return playerData.hardDifficulty;
            default:
                return playerData.easyDifficulty;
        }
    }

    public int GetPlayerRank()
    {
        return currentDifficulty.playerRank;
    }

    public int GetPlayerXP()
    {
        return currentDifficulty.playerXP;
    }

    public void SavePlayerProgress(int rank, int xp, int levelIndex, string word)
    {
        currentDifficulty.playerRank = rank;
        currentDifficulty.playerXP = xp;

        List<string> levelCompleted = getCompletedLevelList(levelIndex);
        if (!levelCompleted.Contains(word))
            levelCompleted.Add(word);

        SavePlayerData();
    }

    public void UnlockNextLevel(int levelIndex)
    {
        currentDifficulty.levelsUnlocked[levelIndex + 1] = true;
        SavePlayerData();
    }

    public void UnlockNormalAndHardDifficulty()
    {
        playerData.difficultyUnlocked[1] = true;
        playerData.difficultyUnlocked[2] = true;
        SavePlayerData();
    }

    public void UpdatePlayerDifficulty(String difficultySelected)
    {
        if (playerData.difficultySelected.Equals(difficultySelected))
            return;
        playerData.difficultySelected = difficultySelected;
        SavePlayerData();
        currentDifficulty = GetCurrentDifficulty();
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(2.0f);
    }
    IEnumerator LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);

        string dataAsJson;
        if (AndroidJar(filePath))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
            // deserialize string into object
            gameData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            if (File.Exists(filePath))
            {
                dataAsJson = File.ReadAllText(filePath);
                // deserialize string into object
                gameData = JsonUtility.FromJson<GameData>(dataAsJson);
            }
            else
            {
                Debug.LogError("Cannot load game data!");
            }
        }
        
    }

    private bool AndroidJar(String filePath)
    {
        return filePath.Contains("://") || filePath.Contains(":///");
    }

    private void LoadSettings()
    {
        if (!PlayerPrefs.HasKey(MUSIC_VOLUME))
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolDefault);
        }
        if (!PlayerPrefs.HasKey(WEAPONS_VOLUME))
        {
            PlayerPrefs.SetFloat(WEAPONS_VOLUME, weaponsVolDefault);
        }
        if (!PlayerPrefs.HasKey(EXPLOSIONS_VOLUME))
        {
            PlayerPrefs.SetFloat(EXPLOSIONS_VOLUME, explosionsVolDefault);
        }
        if (!PlayerPrefs.HasKey(WORDS_VOLUME))
        {
            PlayerPrefs.SetFloat(WORDS_VOLUME, wordsVolDefault);
        }
        if (!PlayerPrefs.HasKey(PICKUPS_VOLUME))
        {
            PlayerPrefs.SetFloat(PICKUPS_VOLUME, pickupsVolDefault);
        }
        if (!PlayerPrefs.HasKey(JOYSTICK_CONTROL))
        {
            PlayerPrefs.SetString(JOYSTICK_CONTROL, JOYSTICK_CONTROL_LEFT);
        }
    }
    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("MainMenu");
        yield return new WaitForSeconds(2.0f);
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
        string dataAsJson = JsonUtility.ToJson(gameData);
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);
        File.WriteAllText(filePath, dataAsJson);
    }

    private void SavePlayerData()
    {
        string dataAsJson = JsonUtility.ToJson(playerData);
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFilename);
        byte[] bytes = Encoding.ASCII.GetBytes(dataAsJson);
        File.WriteAllBytes(filePath, bytes);
    }
}