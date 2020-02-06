using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
/**
 * Description: Regulates the game's access to game and player data, including
 * loading both from the file system and saving player data back to it.
 * 
 * Details:
 * SavePlayerData: Write player data to json file
 * UnlockNormalAndHardDifficulty: Unlock normal and hard difficulty and save changes
 * UpdatePlayerDifficulty: Update player difficulty to new, if changed from current, and save changes
 * Start: Indicate data controller not to be destroyed, then load game and player data, initialize 
 * current difficulty and game settings, before finally loading main menu.
 * DetermineDifficulty: Return difficulty object from player data based on difficulty selected
 * LoadGameSettings: Set volume and control settings to default, if not set
 * LoadMainMenu: Call load scene on main menu
 * LoadPlayerData: Read player data from json file
 * LoadGameData: Read game data from json file
 * IsAndroidJar: Check file path is jar file or not
 */
public class DataController : MonoBehaviour
{
    public GameData gameData;
    public PlayerData playerData;
    public Difficulty currentDifficulty;

    public const string GAME_SCENE = "Game";
    public const string PERSISTENT_SCENE = "Persistent";
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string INFO_SCENE = "Info";
    public const string SETTINGS_SCENE = "Settings";

    public const int RECRUIT_RANK = 0;
    public const int CADET_RANK = 1;
    public const int PILOT_RANK = 2;
    public const int ACE_RANK = 3;
    public const int CHIEF_RANK = 4;
    public const int CAPTAIN_RANK = 5;
    public const int COMMANDER_RANK = 6;
    public const int MASTER_RANK = 7;

    public const int LEVEL_ONE = 0;
    public const int LEVEL_TWO = 1;
    public const int LEVEL_THREE = 2;
    public const int LEVEL_FOUR = 3;
    public const int LEVEL_FIVE = 4;
    public const int LEVEL_SIX = 5;
    public const int LEVEL_SEVEN = 6;
    public const int LEVEL_EIGHT = 7;
    public const int LEVEL_NINE = 8;
    public const int LEVEL_TEN = 9;

    public const string RECRUIT_RANK_TEXT = "Recruit";
    public const string CADET_RANK_TEXT = "Cadet";
    public const string PILOT_RANK_TEXT = "Pilot";
    public const string ACE_RANK_TEXT = "Ace Pilot";
    public const string CHIEF_RANK_TEXT = "Chief";
    public const string CAPTAIN_RANK_TEXT = "Captain";
    public const string COMMANDER_RANK_TEXT = "Commander";
    public const string MASTER_RANK_TEXT = "Master";
    public const string RANK_UNDEFINED_TEXT = "UNDEFINED";

    public const string DIFFICULTY_EASY = "EASY";
    public const string DIFFICULTY_NORMAL = "NORMAL";
    public const string DIFFICULTY_HARD = "HARD";

    public const string MUSIC_VOLUME = "MUSIC_VOL";
    public const string WEAPONS_VOLUME = "WEAPONS_VOL";
    public const string EXPLOSIONS_VOLUME = "EXPLOSIONS_VOL";
    public const string VOICES_VOLUME = "VOICES_VOL";
    
    public const string JOYSTICK_CONTROL = "JOYSTICK_CONTROL";
    public const string JOYSTICK_CONTROL_LEFT = "LEFT-HANDED";
    public const string JOYSTICK_CONTROL_RIGHT = "RIGHT-HANDED";
    public const float DEFAULT_VOL = 0.5f;

    public const string GAME_LEVEL_KEY = "LEVEL";
    public const string PLAYER_HEALTH_KEY = "HAVE_PLAYER_HEALTH";
    public const string PLAYER_STREAK_KEY = "HAVE_PLAYER_STREAK";
    public const string DUALSHOT_KEY = "HAVE_DUALSHOT";
    public const string ARMOR_KEY = "HAVE_ARMOR";
    public const string TELEPORT_KEY = "HAVE_TELEPORT";

    public const string MESSAGES_CONCILLATORY = "Better luck next time! Tap to continue.";
    public const string MESSAGES_CONGRATULATORY = "Good job! Tap to continue.";
    public const string MESSAGES_END_OF_ROUND_STREAK_PLACEHOLDER = "x#";
    public const string MESSAGES_END_OF_ROUND_LEVEL_UNLOCKED_PLACEHOLDER = "LEVEL # UNLOCKED";
    public const string MESSAGES_END_OF_ROUND_RANK_ACHIEVED_PLACEHOLDER = "# RANK ACHIEVED";
    public const string MESSAGES_END_OF_ROUND_NORMAL_HARD_UNLOCKED = "NORMAL & HARD UNLOCKED";
    public const string MESSAGES_END_OF_ROUND_LEVEL_COMPLETED_PLACEHOLDER= "LEVEL COMPLETED +# XP";
    public const string MESSAGES_PICKUPS_HEALTH_HEADER = "HEALTH";
    public const string MESSAGES_PICKUPS_DUALSHOT_HEADER = "DUAL SHOT";
    public const string MESSAGES_PICKUPS_ARMOR_HEADER = "ARMOR";
    public const string MESSAGES_PICKUPS_TELEPORT_HEADER = "TELEPORT";

    public const string MESSAGES_PICKUPS_HEALTH_INFO = "Add 1 point to HP";
    public const string MESSAGES_PICKUPS_DUALSHOT_INFO = "Fire Two Bolts";
    public const string MESSAGES_PICKUPS_ARMOR_INFO = "Absorb Any Damage";
    public const string MESSAGES_PICKUPS_TELEPORT_INFO = "Tap Anywhere And Move";
    public const string MESSAGES_RESUME_GAME = "Tap Screen to Resume";

    private string gameDataFilename = "game.json";
    private string playerDataFilename = "player.json";

    private float musicVolDefault = 0.05f;
    private float weaponsVolDefault = 0.10f;
    private float explosionsVolDefault = 0.15f;
    private float voicesVolDefault = 0.8f;

    public void SavePlayerData()
    {
        string dataAsJson = JsonUtility.ToJson(playerData);
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFilename);
        byte[] bytes = Encoding.ASCII.GetBytes(dataAsJson);
        File.WriteAllBytes(filePath, bytes);
    }
    public void UnlockNormalAndHardDifficulty()
    {
        playerData.difficultyUnlocked[1] = true;
        playerData.difficultyUnlocked[2] = true;
        SavePlayerData();
    }
    public void UpdatePlayerDifficulty(string difficultySelected)
    {
        playerData.difficultySelected = difficultySelected;
        SavePlayerData();
        currentDifficulty = DetermineDifficulty();
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadGameData());
        LoadPlayerData();
        currentDifficulty = DetermineDifficulty();
        LoadGameSettings();
        StartCoroutine(LoadMainMenu());
    }
    private Difficulty DetermineDifficulty()
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
    private void LoadGameSettings()
    {
        if (!PlayerPrefs.HasKey(MUSIC_VOLUME)) PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolDefault);
        if (!PlayerPrefs.HasKey(WEAPONS_VOLUME)) PlayerPrefs.SetFloat(WEAPONS_VOLUME, weaponsVolDefault);
        if (!PlayerPrefs.HasKey(EXPLOSIONS_VOLUME)) PlayerPrefs.SetFloat(EXPLOSIONS_VOLUME, explosionsVolDefault);
        if (!PlayerPrefs.HasKey(VOICES_VOLUME)) PlayerPrefs.SetFloat(VOICES_VOLUME, voicesVolDefault);
        if (!PlayerPrefs.HasKey(JOYSTICK_CONTROL)) PlayerPrefs.SetString(JOYSTICK_CONTROL, JOYSTICK_CONTROL_LEFT);
    }
    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(MAIN_MENU_SCENE);
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
            playerData.easyDifficulty = new Difficulty();
            playerData.easyDifficulty.name = DataController.DIFFICULTY_EASY;
            playerData.normalDifficulty = new Difficulty();
            playerData.normalDifficulty.name = DataController.DIFFICULTY_NORMAL;
            playerData.hardDifficulty = new Difficulty();
            playerData.hardDifficulty.name = DataController.DIFFICULTY_HARD;
            string dataAsJson = JsonUtility.ToJson(playerData);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(dataAsJson);
            File.WriteAllBytes(filePath, jsonBytes);

            byte[] jsonBytesRead = File.ReadAllBytes(filePath);
            string dataAsJsonRead = Encoding.ASCII.GetString(jsonBytesRead);
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJsonRead);
        }
    }
    IEnumerator LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);

        string dataAsJson;
        if (IsAndroidJar(filePath))
        {
            UnityEngine.Networking.UnityWebRequest jarFile = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return jarFile.SendWebRequest();
            dataAsJson = jarFile.downloadHandler.text;
            gameData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else if(File.Exists(filePath))
        {
            dataAsJson = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
    private bool IsAndroidJar(string filePath)
    {
        return filePath.Contains("://") || filePath.Contains(":///");
    }
}