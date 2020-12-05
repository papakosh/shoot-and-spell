using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Manages most of the gameplay, including an endless run of asteroids, letters, and enemy ships; scoring the words 
 * spelled; rewarding the player with rank; spawning pickups, and finally, the player experience, like using skills and taking damage.
 * 
 * Details - 
 * LoadMainMenu - Delete temporary data, unpause the game, and finally load the main menu.
 * PlayWord - Play the audio for the current word 
 * ProcessLetterHit - Evaluate the letter that was destroyed and match it against the next letter in the current word. For non-matches, slow down the player's ship. 
 * For matches, if it is the last letter, end the round; else, increment to next letter.
 * SpawnRandomPickup - Pick a random number between 1 and 6. If the number 3 is picked, spawn a random pickup depending on the player's rank. The first time 
 * a pickup appears, the game pauses and a usage tip displays to the user.
 * ResumeGame - Hide pickup messages, show the player ship and unpause the game
 * LoseRound - Show a concillatory message, delete temporary data, and mark the round as over.
 * PlayAnotherRound - Reload the game on the current level
 * RefreshHealthBar - Show a message of +HP when adding health to the player or -HP when subtracting health from the player, and then update hp bar to reflect 
 * the new value
 * PlayerShipHit - The ship flashes for several seconds, alternating color between red and white, to indicate damage taken after being hit
 * ArmorActive - The ship flashes for several seconds, alternating color between yellow and white, to indicate no damage taken after being hit
 * UpdateTeleportStatusIcon - Change status icon on UI to reflect whether player has teleport skill or not
 * UpdateArmorStatusIcon - Change status icon on UI to reflect whether player has armor skill or not
 * UpdateDualShotStatusIcon - Change status icon on UI to reflect whether player has dual shot skill or not
 * Awake - Instantiate instance of gamecontroller class, locate the data controller object, set the current difficulty, and initialize audio source object
 * Start - Setup the game level (initial level values, background image, debris wait times, word to be spelled); Setup the player data (retrieve player stats
 * and customize player UI options); Setup UI (prepare UI for display); Countdown from 5 seconds; and finally, spawn waves of debris (asteroids, enemy ships, 
 * and letter blocks) when the countdown reaches zero to begin play.
 */
public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    [HideInInspector]
    public bool isGamePaused;
    public GameObject[] hazards;
    public GameObject[] blocks;
    public GameObject[] pickups;
    public Vector3 debrisOrigin;
    public int debrisCount;
    public float debrisSpawnWait, debrisStartWait, debrisWaveWait;
    public float countdown;
    public GameObject gameBackground;
    public GameObject[] letterImages;
    public GameObject[] selectedWordPanel;
    public GameObject[] displayPickupMessage;
    public GameObject[] displayRoundMessages;

    [HideInInspector]
    public bool isPlayerDead;
    public GameObject playerShip;
    public Slider xpBar;
    public Slider hpBar;
    public Text rankText;
    public GameObject xpEarnedText;
    public GameObject hpChangeText;
    public float playerHitFlashWait = 0.125f;
    public float playerSlowDownFlashWait = 0.25f;
    public int numberOfFlashes = 3;
    public GameObject dualShotStatusIcon;
    public GameObject armorStatusIcon;
    public GameObject teleportStatusIcon;
    public GameObject joystickControlLeft;
    public GameObject joystickControlRight;
    public GameObject mainMenuButton;

    private DataController dataController;
    private Difficulty currentDifficulty;
    private AudioSource _audio;

    private int currentGameLevelIndex;
    private int nextGameLevelIndex;
    private bool roundOver;
    private int levelXPModifier = 1;
    private int levelCompleteBonus;
    private bool levelIncomplete;
    private GameObject[] debrisArray;
    private Color letterMatchedColorGold = new Color32(212, 175, 55, 255);
    private int targetLetterIndex; 
    private AudioClip selectedWordClip;
    private int selectedWordIndex;
    private string[] endOfRoundMsgs = { DataController.MESSAGES_END_OF_ROUND_STREAK_PLACEHOLDER, 
        DataController.MESSAGES_END_OF_ROUND_LEVEL_UNLOCKED_PLACEHOLDER,
        DataController.MESSAGES_END_OF_ROUND_RANK_ACHIEVED_PLACEHOLDER, 
        DataController.MESSAGES_END_OF_ROUND_NORMAL_HARD_UNLOCKED, 
        DataController.MESSAGES_END_OF_ROUND_LEVEL_COMPLETED_PLACEHOLDER};
    private string[] pickupHeaders = { DataController.MESSAGES_PICKUPS_HEALTH_HEADER, 
        DataController.MESSAGES_PICKUPS_DUALSHOT_HEADER,
        DataController.MESSAGES_PICKUPS_ARMOR_HEADER,
        DataController.MESSAGES_PICKUPS_TELEPORT_HEADER};
    private string[] pickupMsgs = { DataController.MESSAGES_PICKUPS_HEALTH_INFO, 
        DataController.MESSAGES_PICKUPS_DUALSHOT_INFO,
        DataController.MESSAGES_PICKUPS_ARMOR_INFO,
        DataController.MESSAGES_PICKUPS_TELEPORT_INFO};
    private string resumePlayingMessage = DataController.MESSAGES_RESUME_GAME;
    
    private const int LETTER_A_ASCII = 65;
    private const int ENEMY_HAZARD = 3;
    private const int ASTEROID_OR_ENEMY = 0;
    private const string ACTIVE_STATUS = "ACTIVE";
    private const string INACTIVE_STATUS = "INACTIVE";
    private const string MAXIMUM_RANK_TEXT = "MAXED";
    private const int DELAY = 3;
    private const int LEVEL_ONE = 0;
    private const int LEVEL_TWO = 1;
    private const int LEVEL_THREE = 2;
    private const int LEVEL_FOUR = 3;
    private const int LEVEL_FIVE = 4;
    private const int LEVEL_SIX = 5;
    private const int LEVEL_SEVEN = 6;
    private const int LEVEL_EIGHT = 7;
    private const int LEVEL_NINE = 8;
    private const int LEVEL_TEN = 9;
    private const float playerStreakAdditive = 0.05f;
    private const int HEALTH_PICKUP = 0;
    private const int DUALSHOT_PICKUP = 1;
    private const int ARMOR_PICKUP = 2;
    private const int TELEPORT_PICKUP = 3;
    
    private const string RESOURCES_UI_HEALTH_ICON = "Sprites/Pickups/health_icon";
    private const string RESOURCES_UI_DUALSHOT_ICON = "Sprites/Pickups/dual_shot_icon";
    private const string RESOURCES_UI_ARMOR_ICON = "Sprites/Pickups/armor_icon";
    private const string RESOURCES_UI_TELEPORT_ICON = "Sprites/Pickups/teleport_icon";
    private const string RESOURCES_UI_GAME_PANEL_ACTIVE = "Sprites/UI/Game/panel_active";
    private const string RESOURCES_UI_GAME_PANEL_INACTIVE = "Sprites/UI/Game/panel";
    private const string RESOURCES_UI_GAME_MAINMENU_ACTIVE = "Sprites/UI/Game/main_menu_active";
    private const string RESOURCES_UI_GAME_MAINMENU_INACTIVE = "Sprites/UI/Game/main_menu";

    private const string EASY_SEEN_HEALTH_PICKUP_KEY = "EASY_SEEN_HEALTH";
    private const string EASY_SEEN_DUALSHOT_PICKUP_KEY = "EASY_SEEN_DUALSHOT";
    private const string EASY_SEEN_ARMOR_PICKUP_KEY = "EASY_SEEN_ARMOR";
    private const string EASY_SEEN_TELEPORT_PICKUP_KEY = "EASY_SEEN_TELEPORT";

    private const string NORMAL_SEEN_HEALTH_PICKUP_KEY = "NORMAL_SEEN_HEALTH";
    private const string NORMAL_SEEN_DUALSHOT_PICKUP_KEY = "NORMAL_SEEN_DUALSHOT";
    private const string NORMAL_SEEN_ARMOR_PICKUP_KEY = "NORMAL_SEEN_ARMOR";
    private const string NORMAL_SEEN_TELEPORT_PICKUP_KEY = "NORMAL_SEEN_TELEPORT";

    private const string HARD_SEEN_HEALTH_PICKUP_KEY = "HARD_SEEN_HEALTH";
    private const string HARD_SEEN_DUALSHOT_PICKUP_KEY = "HARD_SEEN_DUALSHOT";
    private const string HARD_SEEN_ARMOR_PICKUP_KEY = "HARD_SEEN_ARMOR";
    private const string HARD_SEEN_TELEPORT_PICKUP_KEY = "HARD_SEEN_TELEPORT";

    private const string MINUS = "-";
    private const string PLUS = "+";
    private const string PLACEHOLDER = "#";
    private const string EMPTY_STRING = "";
    private const string KEYVALUE_FILLED = "YES";

    private const string HIT_POINTS = " HP";

    private string selectedWord;
    private int playerRank;
    private int playerXP;
    private int xpPlayerEarned;
    private bool playerUnlockedNextLevel = false;
    private bool playerUnlockedNormalHardDifficulty = false;
    private bool playerAchievedNextRank = false;
    private bool hasPlayerCompletedLevel = false;
    private Color shipNormalColorWhite = Color.white;
    private Color shipHitColorRed = Color.red;
    private Color shipHitAbsorbedColorYellow = Color.yellow;

    private float playerStreak = 0f;

    public void LoadMainMenu()
    {
        PlayerPrefs.DeleteKey(DataController.PLAYER_HEALTH_KEY);
        PlayerPrefs.DeleteKey(DataController.PLAYER_STREAK_KEY);
        PlayerPrefs.DeleteKey(DataController.DUALSHOT_KEY);
        PlayerPrefs.DeleteKey(DataController.ARMOR_KEY);
        PlayerPrefs.DeleteKey(DataController.TELEPORT_KEY);
        Time.timeScale = 1f;
        isGamePaused = false;

        SceneManager.LoadScene(DataController.MAIN_MENU_SCENE);
    }
    public void PlayWord()
    {
        _audio.clip = selectedWordClip;
        _audio.volume = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        _audio.Play();
    }
    public void ProcessLetterHit(string hitLetter)
    {
        string targetLetter;
        if (IsLastLetter()) targetLetter = selectedWord.Substring(targetLetterIndex);
        else targetLetter = selectedWord.Substring(targetLetterIndex, 1);

        if (LettersMatch(targetLetter, hitLetter))
        {
            selectedWordPanel[targetLetterIndex].SetActive(true);
            selectedWordPanel[targetLetterIndex].GetComponent<Image>().color = letterMatchedColorGold;

            if (IsLastLetter()) RoundWon();
            else targetLetterIndex++;
        }
        else StartCoroutine(SlowPlayerShip());
    }
    public void SpawnRandomPickup(Transform pickupTransform)
    {
        int randomNum = UnityEngine.Random.Range(1, 6);
        switch (randomNum)
        {
            case 1:
            case 2:
                break;
            case 3:
                int pickupChosen = 0;
                if (playerRank > DataController.CHIEF_RANK) pickupChosen = UnityEngine.Random.Range(0, 4);
                else if (playerRank > DataController.PILOT_RANK) pickupChosen = UnityEngine.Random.Range(0, 3);
                else if (playerRank > DataController.CADET_RANK) pickupChosen = UnityEngine.Random.Range(0, 2);

                Quaternion rotateQuaternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
                if (IsFirstTimeForPickup(pickupChosen))
                {
                    Instantiate(pickups[pickupChosen], pickupTransform.position, rotateQuaternion);
                    if (IsEasyDifficulty())
                    {
                        if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(EASY_SEEN_HEALTH_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(EASY_SEEN_DUALSHOT_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(EASY_SEEN_ARMOR_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(EASY_SEEN_TELEPORT_PICKUP_KEY, KEYVALUE_FILLED);
                    }else if (IsNormalDifficulty())
                    {
                        if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_HEALTH_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_DUALSHOT_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_ARMOR_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_TELEPORT_PICKUP_KEY, KEYVALUE_FILLED);
                    }
                    else if (IsHardDifficulty())
                    {
                        if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(HARD_SEEN_HEALTH_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(HARD_SEEN_DUALSHOT_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(HARD_SEEN_ARMOR_PICKUP_KEY, KEYVALUE_FILLED);
                        else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(HARD_SEEN_TELEPORT_PICKUP_KEY, KEYVALUE_FILLED);
                    }

                    displayPickupMessage[0].GetComponent<Image>().sprite = GetPickupSprite(pickupChosen);
                    displayPickupMessage[1].GetComponent<Text>().text = pickupHeaders[pickupChosen];
                    displayPickupMessage[2].GetComponent<Text>().text = pickupMsgs[pickupChosen];
                    displayPickupMessage[3].GetComponent<Text>().text = resumePlayingMessage;
                    StartCoroutine(ShowPickupMsg());
                }
                else
                    Instantiate(pickups[pickupChosen], pickupTransform.position, rotateQuaternion);
                break;
            case 4:
            case 5:
            case 6:
                break;
            default: break;
        }
    }
    public void ResumeGame()
    {
        HidePickupMessage();
        ShowPlayerShip();
        Time.timeScale = 1f;
        isGamePaused = false;
    }
    public void LoseRound()
    {
        ShowRoundLostMessage();

        PlayerPrefs.DeleteKey(DataController.PLAYER_HEALTH_KEY);
        PlayerPrefs.DeleteKey(DataController.PLAYER_STREAK_KEY);
        PlayerPrefs.DeleteKey(DataController.DUALSHOT_KEY);
        PlayerPrefs.DeleteKey(DataController.ARMOR_KEY);
        PlayerPrefs.DeleteKey(DataController.TELEPORT_KEY);

        roundOver = true;
    }
    public void PlayAnotherRound()
    {
        SceneManager.LoadScene(DataController.GAME_SCENE);
    }
    public void RefreshHealthBar(float amt, bool isDamaged)
    {
        StartCoroutine(DisplayHPChangeText(DELAY, amt, isDamaged));

        hpBar.maxValue = PlayerController.instance.maxHealth;
        hpBar.value = PlayerController.instance.currentHealth;
    }
    public IEnumerator PlayerShipHit()
    {
        for (int i = 1; i <= numberOfFlashes; i++)
        {
            if (IsPlayerShipActive())
            {
                playerShip.GetComponent<Renderer>().material.color = shipHitColorRed;
                yield return new WaitForSeconds(playerHitFlashWait);
            }
            if (IsPlayerShipActive())
            {
                playerShip.GetComponent<Renderer>().material.color = shipNormalColorWhite;
                yield return new WaitForSeconds(playerHitFlashWait);
            }
        }
    }
    public IEnumerator ArmorActive()
    {
        for (int i = 1; i <= numberOfFlashes; i++)
        {
            playerShip.GetComponent<Renderer>().material.color = shipHitAbsorbedColorYellow;
            yield return new WaitForSeconds(playerHitFlashWait);
            playerShip.GetComponent<Renderer>().material.color = shipNormalColorWhite;
            yield return new WaitForSeconds(playerHitFlashWait);
        }
    }
    public void UpdateTeleportStatusIcon(string newStatus)
    {
        teleportStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(newStatus);
    }
    public void UpdateArmorStatusIcon(string newStatus)
    {
        armorStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(newStatus);
    }
    public void UpdateDualShotStatusIcon(string newStatus)
    {
        dualShotStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(newStatus);
    }
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        dataController = FindObjectOfType<DataController>();
        currentDifficulty = dataController.currentDifficulty;
        _audio = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupGameLevel();

        SetupPlayerData();

        SetupUI();

        StartCoroutine(CountdownBeforePlay(countdown));

        StartCoroutine(SpawnWaves());
    }
    private void SetupGameLevel()
    {
        currentGameLevelIndex = PlayerPrefs.GetInt(DataController.GAME_LEVEL_KEY);
        nextGameLevelIndex = currentGameLevelIndex + 1;
        roundOver = false;
        levelXPModifier = currentGameLevelIndex + 1;
        levelCompleteBonus = dataController.gameData.allLevelData[currentGameLevelIndex].completionBonus;

        if (!AllWordsSpelt()) levelIncomplete = true;

        CustomizeLevelBackground();

        CustomizeLevelDebrisWaitTimes();

        SetupWordSelected();

        ShowCountdownMessage();
    }
    private void SetupPlayerData()
    {
        playerXP = currentDifficulty.playerXP;
        playerRank = currentDifficulty.playerRank;
        PlayerController.instance.maxHealth = GetMaxHealth();

        if (PlayerPrefs.HasKey(DataController.PLAYER_HEALTH_KEY)) PlayerController.instance.currentHealth = PlayerPrefs.GetFloat(DataController.PLAYER_HEALTH_KEY);
        else PlayerController.instance.currentHealth = PlayerController.instance.maxHealth;

        if (PlayerPrefs.HasKey(DataController.PLAYER_STREAK_KEY)) playerStreak = PlayerPrefs.GetFloat(DataController.PLAYER_STREAK_KEY);
        else playerStreak = 0f;

        if (PlayerPrefs.HasKey(DataController.DUALSHOT_KEY))
        {
            PlayerController.instance.canFireDualShot = true;
            dualShotStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(ACTIVE_STATUS);
        }
        if (PlayerPrefs.HasKey(DataController.ARMOR_KEY))
        {
            PlayerController.instance.canAbsorbDamage = true;
            armorStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(ACTIVE_STATUS);
        }
        if (PlayerPrefs.HasKey(DataController.TELEPORT_KEY))
        {
            PlayerController.instance.canTeleport = true;
            teleportStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(ACTIVE_STATUS);
        }

        if (PlayerPrefs.GetString(DataController.JOYSTICK_CONTROL).Equals(DataController.JOYSTICK_CONTROL_LEFT))
        {
            joystickControlRight.SetActive(false);
            joystickControlLeft.SetActive(true);
        }
        else
        {
            joystickControlLeft.SetActive(false);
            joystickControlRight.SetActive(true);
        }

        HidePlayerShip();
    }
    private void SetupUI()
    {
        mainMenuButton.GetComponent<Button>().interactable = false;

        rankText.text = GetRankText(playerRank);
        if (!HasPlayerReachedMaxRank())
        {
            xpBar.maxValue = (int)CalculateXPForNextRank(playerRank);
            xpBar.value = playerXP;
        }
        else
        {
            xpBar.maxValue = 1;
            xpBar.value = 1;
            xpEarnedText.GetComponent<Text>().text = MAXIMUM_RANK_TEXT;
            xpEarnedText.SetActive(true);
        }
        hpBar.maxValue = PlayerController.instance.maxHealth;
        hpBar.value = PlayerController.instance.currentHealth;
    }
    IEnumerator CountdownBeforePlay(float seconds)
    {
        float counter = seconds;
        while (counter >= 0)
        {
            yield return new WaitForSeconds(1);
            displayRoundMessages[5].GetComponent<Text>().text = counter + EMPTY_STRING;
            counter--;
        }
        HideCountdownMessage();
        ShowPlayerShip();
        Time.timeScale = 1f;
        isGamePaused = false;
    }
    private IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(debrisStartWait + countdown);
        StartCoroutine(DisplayWord(10.0f));

        PlayWord();
        while (true)
        {
            PopulateDebrisArray();

            for (int i = 0; i < debrisCount; i++)
            {
                GameObject debris = debrisArray[i];
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-debrisOrigin.x, debrisOrigin.x),
                    debrisOrigin.y,
                    debrisOrigin.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(debris, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(debrisSpawnWait);
            }
            yield return new WaitForSeconds(debrisWaveWait);

            if (roundOver) break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGamePaused)
            {
                mainMenuButton.GetComponent<Image>().sprite = GetMainMenuStatusSprite(ACTIVE_STATUS);
                mainMenuButton.GetComponent<Button>().interactable = true;
                Time.timeScale = 0f;
                isGamePaused = true;
            }
            else
            {
                mainMenuButton.GetComponent<Image>().sprite = GetMainMenuStatusSprite(INACTIVE_STATUS);
                mainMenuButton.GetComponent<Button>().interactable = false;
                HidePickupMessage();
                Time.timeScale = 1f;
                isGamePaused = false;
            }
        }
        if (!_audio.isPlaying)
            _audio.volume = DataController.DEFAULT_VOL;
    }
    private IEnumerator SlowPlayerShip()
    {
        PlayerController.instance.DecreaseSpeed();
        for (int i = 1; i <= numberOfFlashes * 2; i++)
        {
            if (IsPlayerShipActive())
            {
                playerShip.transform.GetChild(0).gameObject.SetActive(false);
                yield return new WaitForSeconds(playerSlowDownFlashWait);
            }

            if (IsPlayerShipActive())
            {
                playerShip.transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(playerSlowDownFlashWait);
            }
        }
        PlayerController.instance.NormalizeSpeed();
    }
    private string GetRankText(int rank)
    {
        switch (rank)
        {
            case DataController.RECRUIT_RANK:
                return DataController.RECRUIT_RANK_TEXT;
            case DataController.CADET_RANK:
                return DataController.CADET_RANK_TEXT;
            case DataController.PILOT_RANK:
                return DataController.PILOT_RANK_TEXT;
            case DataController.ACE_RANK:
                return DataController.ACE_RANK_TEXT;
            case DataController.CHIEF_RANK:
                return DataController.CHIEF_RANK_TEXT;
            case DataController.CAPTAIN_RANK:
                return DataController.CAPTAIN_RANK_TEXT;
            case DataController.COMMANDER_RANK:
                return DataController.COMMANDER_RANK_TEXT;
            case DataController.MASTER_RANK:
                return DataController.MASTER_RANK_TEXT;
            default:
                return DataController.RANK_UNDEFINED_TEXT;
        }
    }
    private IEnumerator DisplayHPChangeText(int delay, float amt, bool isDamaged)
    {
        ShowHPChangeText();

        if (isDamaged) hpChangeText.GetComponent<Text>().text = MINUS + amt + HIT_POINTS;
        else hpChangeText.GetComponent<Text>().text = PLUS + amt + HIT_POINTS;

        hpChangeText.GetComponent<Text>().CrossFadeAlpha(0, 3.0f, false);
        yield return new WaitForSeconds(delay);
        hpChangeText.GetComponent<Text>().CrossFadeAlpha(1, 0.0f, false);

        HideHPChangedText();
    }
    private void IncreaseStats()
    {
        PlayerController.instance.maxHealth = GetMaxHealth();
        PlayerController.instance.currentHealth = PlayerController.instance.maxHealth;
        PlayerPrefs.DeleteKey(DataController.PLAYER_HEALTH_KEY);
    }
    private bool IsFirstTimeForPickup(int pickupNumber)
    {
        switch (pickupNumber)
        {
            case HEALTH_PICKUP:
                if (IsEasyDifficulty())return !PlayerPrefs.HasKey(EASY_SEEN_HEALTH_PICKUP_KEY);
                else if (IsNormalDifficulty()) return !PlayerPrefs.HasKey(NORMAL_SEEN_HEALTH_PICKUP_KEY);
                else if (IsHardDifficulty()) return !PlayerPrefs.HasKey(HARD_SEEN_HEALTH_PICKUP_KEY);
                break;
            case DUALSHOT_PICKUP:
                if (IsEasyDifficulty()) return !PlayerPrefs.HasKey(EASY_SEEN_DUALSHOT_PICKUP_KEY);
                else if (IsNormalDifficulty()) return !PlayerPrefs.HasKey(NORMAL_SEEN_DUALSHOT_PICKUP_KEY);
                else if (IsHardDifficulty()) return !PlayerPrefs.HasKey(HARD_SEEN_DUALSHOT_PICKUP_KEY);
                break;
            case ARMOR_PICKUP:
                if (IsEasyDifficulty()) return !PlayerPrefs.HasKey(EASY_SEEN_ARMOR_PICKUP_KEY);
                else if (IsNormalDifficulty()) return !PlayerPrefs.HasKey(NORMAL_SEEN_ARMOR_PICKUP_KEY);
                else if (IsHardDifficulty()) return !PlayerPrefs.HasKey(HARD_SEEN_ARMOR_PICKUP_KEY);
                break;
            case TELEPORT_PICKUP:
                if (IsEasyDifficulty()) return !PlayerPrefs.HasKey(EASY_SEEN_TELEPORT_PICKUP_KEY);
                else if (IsNormalDifficulty()) return !PlayerPrefs.HasKey(NORMAL_SEEN_TELEPORT_PICKUP_KEY);
                else if (IsHardDifficulty()) return !PlayerPrefs.HasKey(HARD_SEEN_TELEPORT_PICKUP_KEY);
                break;
        }
        return true;
    }
    private float GetMaxHealth()
    {
        switch (playerRank)
        {
            case DataController.RECRUIT_RANK: return 1.0f;
            case DataController.CADET_RANK: return 2.0f;
            case DataController.PILOT_RANK: return 3.0f;
            case DataController.ACE_RANK: return 4.0f;
            case DataController.CHIEF_RANK: return 5.0f;
            case DataController.CAPTAIN_RANK: return 6.0f;
            case DataController.COMMANDER_RANK: return 7.0f;
            case DataController.MASTER_RANK: return 8.0f;
            default: return 1.0f;
        }
    }
    private double CalculateXPForNextRank(int rank)
    {
        double exponent = dataController.gameData.xpModifier;
        double baseXP = dataController.gameData.baseXP;
        return Math.Floor(baseXP * Math.Pow(rank + 1, exponent));
    }
    private void CheckProgression()
    {
        currentDifficulty.AddToListOfLevelCompletedWords(currentGameLevelIndex, selectedWord);
        if (!HasPlayerReachedMaxRank())
        {
            int nextRankXP = (int)CalculateXPForNextRank(playerRank);
            if (playerXP > nextRankXP)
            {
                playerRank++;
                playerXP -= nextRankXP;
                IncreaseStats();
                playerAchievedNextRank = true;
            }
            currentDifficulty.playerXP = playerXP;
            currentDifficulty.playerRank = playerRank;
        }
        else
        {
            currentDifficulty.playerXP = 0;
            currentDifficulty.playerRank = playerRank;
        }
        
        if (HasUnlockedNextLevel())
        {
            currentDifficulty.levelsUnlocked[nextGameLevelIndex] = true;
            playerUnlockedNextLevel = true;
        } else if (HasSpeltAllLevelWords())
        {
            playerXP += levelCompleteBonus;
            hasPlayerCompletedLevel = true;
            if (!HasPlayerReachedMaxRank())
            {
                int currentRankXP = (int)CalculateXPForNextRank(playerRank);
                if (playerXP > currentRankXP)
                {
                    playerRank++;
                    playerXP -= currentRankXP;
                    IncreaseStats();
                    playerAchievedNextRank = true;
                }
                currentDifficulty.playerXP = playerXP;
                currentDifficulty.playerRank = playerRank;
            }
            else
            {
                currentDifficulty.playerXP = 0;
                currentDifficulty.playerRank = playerRank;
            }
        }
        else
        {
            if (CanUnlockNormalAndHardDifficulty())
            {
               
                dataController.UnlockNormalAndHardDifficulty();
                playerUnlockedNormalHardDifficulty = true;
            }
        }

        dataController.SavePlayerData();
    }
    private string RandomWord()
    {
        WordData[] words = dataController.gameData.allLevelData[currentGameLevelIndex].words;
        string wordChosen = EMPTY_STRING;
        bool foundWord = false;
        int counter = 0;
        while (counter < 10 && !foundWord)
        {
            selectedWordIndex = UnityEngine.Random.Range(0, words.Length);
            wordChosen = words[selectedWordIndex].text;
            if (ListDoesNotContainWord(wordChosen)) {foundWord = true;}
            else counter++;
        }

        return wordChosen;
    }
    private void CalculateWordScore()
    {
        double xpEarned = Math.Round(selectedWord.Length * (double)(levelXPModifier + playerStreak), MidpointRounding.AwayFromZero);
        xpPlayerEarned = (int)xpEarned;
        playerXP += xpPlayerEarned;
    }
    private IEnumerator EndOfRoundStats()
    {
        if (!HasPlayerReachedMaxRank()) ShowXPEarnedMessage();

        ShowPlayerStreakMessage();
        yield return new WaitForSeconds(0.5f);

        if (playerAchievedNextRank && !playerUnlockedNextLevel && !playerUnlockedNormalHardDifficulty && !hasPlayerCompletedLevel)
        {
            ShowRankAchievedMessage();
            yield return new WaitForSeconds(0.5f);
        } else if (playerAchievedNextRank && (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel))
        {
            ShowRankAchievedMessage();
            if (playerUnlockedNextLevel) ShowUnlockedNextLevelMessage();
            else if (playerUnlockedNormalHardDifficulty) ShowUnlockedDifficultyMessage();
            else if (hasPlayerCompletedLevel)ShowLevelCompleteMessage();
            yield return new WaitForSeconds(0.5f);
        }
        else if (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel)
        {
            if (playerUnlockedNextLevel) ShowUnlockedNextLevelMessage2();
            else if (playerUnlockedNormalHardDifficulty) ShowUnlockedDifficultyMessage2();
            else if (hasPlayerCompletedLevel) ShowLevelCompleteMessage2();
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void RoundWon()
    {
        HidePlayerShip();
        ShowRoundWonMessage();
        CalculateWordScore();
        CheckProgression();
        StartCoroutine(EndOfRoundStats());

        PlayerPrefs.SetFloat(DataController.PLAYER_STREAK_KEY, playerStreak + playerStreakAdditive);
        if (PlayerController.instance.canFireDualShot) PlayerPrefs.SetString(DataController.DUALSHOT_KEY, "YES");
        else PlayerPrefs.DeleteKey(DataController.DUALSHOT_KEY);
        if (PlayerController.instance.canAbsorbDamage) PlayerPrefs.SetString(DataController.ARMOR_KEY, "YES");
        else PlayerPrefs.DeleteKey(DataController.ARMOR_KEY);
        if (PlayerController.instance.canTeleport) PlayerPrefs.SetString(DataController.TELEPORT_KEY, "YES");
        else PlayerPrefs.DeleteKey(DataController.TELEPORT_KEY);

        roundOver = true;
    }
    private void PopulateDebrisArray()
    {
        debrisArray = new GameObject[debrisCount];
        GameObject[] randomBlocksArray = new GameObject[8];
        int enemyShipsAllowed = DetermineNumberOfEnemyShipsAllowed();
        int numHazards = DetermineNumberOfHazards();

        for (int j = 0; j < randomBlocksArray.Length; j++)
        {
            int randomNum = UnityEngine.Random.Range(0, 26);
            randomBlocksArray[j] = blocks[randomNum];
        }

        for (int i = 0; i < debrisCount; i++)
        {
            int randomDebrisNum = UnityEngine.Random.Range(0, 3);
            if (randomDebrisNum == ASTEROID_OR_ENEMY)
            {
                int chooseHazard = UnityEngine.Random.Range(0, numHazards);
                if (chooseHazard == ENEMY_HAZARD)
                {
                    if (enemyShipsAllowed > 0)
                    {
                        debrisArray[i] = hazards[chooseHazard];
                        enemyShipsAllowed--;
                    }
                    else debrisArray[i] = hazards[UnityEngine.Random.Range(0, 3)];
                }
                else debrisArray[i] = hazards[chooseHazard];
            }
            else
            {
                int randomBlockNum = UnityEngine.Random.Range(0, 100);
                if (randomBlockNum <= 44) debrisArray[i] = PickALetterBlock (targetLetterIndex);
                else if (BetweenNumbers (randomBlockNum, 44, 54))
                {
                    if (MoreThanOneLetterLeft()) debrisArray[i] = PickALetterBlock (targetLetterIndex+1);
                    else debrisArray[i] = randomBlocksArray[0];
                }
                else if (BetweenNumbers(randomBlockNum, 54, 64))
                {
                    if (MoreThanOneLetterLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 1);
                    else debrisArray[i] = randomBlocksArray[1];
                }
                else if (BetweenNumbers(randomBlockNum, 64, 74))
                {
                    if (MoreThanTwoLettersLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 2);
                    else debrisArray[i] = randomBlocksArray[2];
                }
                else if (BetweenNumbers(randomBlockNum, 74, 79))
                {
                    if (MoreThanTwoLettersLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 2);
                    else debrisArray[i] = randomBlocksArray[3];
                }
                else if (BetweenNumbers(randomBlockNum, 79, 84))
                {
                    if (MoreThanThreeLettersLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 3);
                    else debrisArray[i] = randomBlocksArray[4];
                }
                else if (BetweenNumbers(randomBlockNum, 84, 89))
                {
                    if (MoreThanThreeLettersLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 3);
                    else debrisArray[i] = randomBlocksArray[5];
                }
                else if (BetweenNumbers(randomBlockNum, 89, 94))
                {
                    if (MoreThanFourLettersLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 4);
                    else debrisArray[i] = randomBlocksArray[6];
                }
                else if (randomBlockNum > 94)
                {
                    if (MoreThanFourLettersLeft()) debrisArray[i] = PickALetterBlock(targetLetterIndex + 4);
                    else debrisArray[i] = randomBlocksArray[7];
                }
            }
        }
    }
    private int ConvertCharacterToAscii(char letter)
    {
        return letter;
    }

    private IEnumerator DisplayWord(float delay)
    {
        if (!IsHardDifficulty())
        {
            char[] letters = selectedWord.ToCharArray();
            for (int i = 0; i < selectedWord.Length; i++)
            {
                int letterAscii = ConvertCharacterToAscii(letters[i]);
                int letterIndex = letterAscii - LETTER_A_ASCII;

                selectedWordPanel[i].GetComponent<Image>().sprite = letterImages[letterIndex].GetComponent<Image>().sprite;
                selectedWordPanel[i].SetActive(true);
            }
            if (IsNormalDifficulty())
            {
                yield return new WaitForSeconds(delay);
                for (int i = 0; i < selectedWordPanel.Length; i++)
                {
                    if (selectedWordPanel[i].GetComponent<Image>().color != letterMatchedColorGold) selectedWordPanel[i].SetActive(false); 
                }
            }
        }
        else
        {
            char[] letters = selectedWord.ToCharArray();
            for (int i = 0; i < selectedWord.Length; i++)
            {
                int letterAscii = ConvertCharacterToAscii(letters[i]);
                int letterIndex = letterAscii - LETTER_A_ASCII;
                selectedWordPanel[i].GetComponent<Image>().sprite = letterImages[letterIndex].GetComponent<Image>().sprite;
                selectedWordPanel[i].SetActive(false);
            }
        }
    }
    private void OnDestroy()
    {
        if (!isPlayerDead) PlayerPrefs.SetFloat(DataController.PLAYER_HEALTH_KEY, PlayerController.instance.currentHealth);
    }
    private void HidePlayerShip()
    {
        playerShip.SetActive(false);
    }
    private void ShowPlayerShip()
    {
        playerShip.SetActive(true);
    }
    private void HidePickupMessage()
    {
        displayPickupMessage[0].SetActive(false);
        displayPickupMessage[1].SetActive(false);
        displayPickupMessage[2].SetActive(false);
        displayPickupMessage[3].SetActive(false);
    }

    private IEnumerator ShowPickupMsg()
    {
        yield return new WaitForSeconds(0.75f);
        displayPickupMessage[0].SetActive(true);
        displayPickupMessage[1].SetActive(true);
        displayPickupMessage[2].SetActive(true);
        displayPickupMessage[3].SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    private void ShowRoundLostMessage()
    {
        displayRoundMessages[6].GetComponent<Text>().text = DataController.MESSAGES_CONCILLATORY;
        displayRoundMessages[6].SetActive(true);
    }
    private void ShowRoundWonMessage()
    {
        displayRoundMessages[6].GetComponent<Text>().text = DataController.MESSAGES_CONGRATULATORY;
        displayRoundMessages[6].SetActive(true);
    }
    private bool IsLastLetter()
    {
        return targetLetterIndex == selectedWord.Length - 1;
    }
    private bool LettersMatch(string targetLetter, string hitLetter)
    {
        return targetLetter.Equals(hitLetter);
    }
    private Sprite GetPickupSprite(int pickupChosen)
    {
        switch (pickupChosen)
        {
            case HEALTH_PICKUP:
                return Resources.Load<Sprite>(RESOURCES_UI_HEALTH_ICON);
            case DUALSHOT_PICKUP:
                return Resources.Load<Sprite>(RESOURCES_UI_DUALSHOT_ICON);
            case ARMOR_PICKUP:
                return Resources.Load<Sprite>(RESOURCES_UI_ARMOR_ICON);
            case TELEPORT_PICKUP:
                return Resources.Load<Sprite>(RESOURCES_UI_TELEPORT_ICON);
            default: return null;
        }
    }

    private bool IsPlayerShipActive()
    {
        return playerShip != null && playerShip.activeSelf;
    }
    private Sprite GetSkillStatusSprite(string status)
    {
        return ACTIVE_STATUS.Equals(status) ? Resources.Load<Sprite>(RESOURCES_UI_GAME_PANEL_ACTIVE) : Resources.Load<Sprite>(RESOURCES_UI_GAME_PANEL_INACTIVE);
    }
    private Sprite GetMainMenuStatusSprite(string status)
    {
        return ACTIVE_STATUS.Equals(status) ? Resources.Load<Sprite>(RESOURCES_UI_GAME_MAINMENU_ACTIVE) : Resources.Load<Sprite>(RESOURCES_UI_GAME_MAINMENU_INACTIVE);
    }
    private void ShowCountdownMessage()
    {
        displayRoundMessages[5].SetActive(true);
    }
    private void HideCountdownMessage()
    {
        displayRoundMessages[5].SetActive(false);
    }
    private Texture GetBackGroundTexture(int index)
    {
        return Resources.Load<Texture>(dataController.gameData.allLevelData[index].backgroundPath);
    }
    private void CustomizeLevelBackground()
    {
        Texture nebulaBackground = GetBackGroundTexture(currentGameLevelIndex);
        gameBackground.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
        GameObject backgroundChild = gameBackground.transform.GetChild(0).gameObject;
        backgroundChild.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
    }
    private void SetupWordSelected()
    {
        targetLetterIndex = 0;
        selectedWord = RandomWord();
        selectedWordClip = Resources.Load<AudioClip>(dataController.gameData.allLevelData[currentGameLevelIndex].words[selectedWordIndex].audioPath);
        _audio.clip = selectedWordClip;
    }

    private bool AllWordsSpelt()
    {
        return currentDifficulty.ListOfLevelCompletedWords(currentGameLevelIndex).Count == dataController.gameData.allLevelData[currentGameLevelIndex].words.Length;
    }
    private void CustomizeLevelDebrisWaitTimes()
    {
        debrisSpawnWait = dataController.gameData.spawnWait - (dataController.gameData.spawnWaitDecrement * (currentGameLevelIndex + 1));
        switch (currentGameLevelIndex)
        {
            case DataController.LEVEL_ONE:
            case DataController.LEVEL_TWO:
            case DataController.LEVEL_THREE:
            case DataController.LEVEL_FOUR:
            case DataController.LEVEL_FIVE:
            case DataController.LEVEL_SIX:
            case DataController.LEVEL_SEVEN:
                debrisWaveWait = dataController.gameData.waveWait - 0;
                break;
            case DataController.LEVEL_EIGHT:
            case DataController.LEVEL_NINE:
                debrisWaveWait = dataController.gameData.waveWait - 1;
                break;
            case DataController.LEVEL_TEN:
                debrisWaveWait = dataController.gameData.waveWait - 2;
                break;
        }
    }
    private void ShowPlayerStreakMessage()
    {
        displayRoundMessages[0].SetActive(true);
        displayRoundMessages[0].GetComponent<Image>().CrossFadeAlpha(0, 9.0f, true);

        float playerStreakCount = playerStreak / 0.05f;
        displayRoundMessages[1].GetComponent<Text>().text = endOfRoundMsgs[0].Replace(PLACEHOLDER, EMPTY_STRING + ((int)playerStreakCount + 1));
        displayRoundMessages[1].SetActive(true);
        displayRoundMessages[1].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowRankAchievedMessage()
    {
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[2].Replace(PLACEHOLDER, EMPTY_STRING + GetRankText(playerRank).ToUpper());
        displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowUnlockedNextLevelMessage()
    {
        string nextGameLevelNumber = (nextGameLevelIndex + 1).ToString();
        displayRoundMessages[3].SetActive(true);
        displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[1].Replace(PLACEHOLDER, EMPTY_STRING + nextGameLevelNumber);
        displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }

    private void ShowUnlockedNextLevelMessage2()
    {
        string nextGameLevelNumber = (nextGameLevelIndex + 1).ToString();
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[1].Replace(PLACEHOLDER, EMPTY_STRING + nextGameLevelNumber);
        displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }

    private void ShowUnlockedDifficultyMessage()
    {
        displayRoundMessages[3].SetActive(true);
        displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[3];
        displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowUnlockedDifficultyMessage2()
    {
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[3];
        displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }

    private void ShowLevelCompleteMessage()
    {
        displayRoundMessages[3].SetActive(true);
        displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[4].Replace(PLACEHOLDER, EMPTY_STRING + levelCompleteBonus);
        displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowLevelCompleteMessage2()
    {
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[4].Replace(PLACEHOLDER, EMPTY_STRING + levelCompleteBonus);
        displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowXPEarnedMessage()
    {
        xpEarnedText.SetActive(true);
        xpEarnedText.GetComponent<Text>().text = "+" + xpPlayerEarned + " xp ";
        xpEarnedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowHPChangeText()
    {
        hpChangeText.SetActive(true);
    }
    private void HideHPChangedText()
    {
        hpChangeText.SetActive(false);
    }
    private bool HasPlayerReachedMaxRank()
    {
        return playerRank == DataController.MASTER_RANK;
    }
    private bool HasUnlockedNextLevel()
    {
        return currentGameLevelIndex < DataController.LEVEL_TEN && NextLevelIsLocked() && SpeltAtleast51PercentOfLevelWords();
    }
    private bool NextLevelIsLocked()
    {
        return !currentDifficulty.levelsUnlocked[currentGameLevelIndex + 1];
    }
    private bool SpeltAtleast51PercentOfLevelWords()
    {
        return currentDifficulty.ListOfLevelCompletedWords(currentGameLevelIndex).Count >= ((dataController.gameData.allLevelData[currentGameLevelIndex].words.Length / 2) + 1);
    }
    private bool HasSpeltAllLevelWords()
    {
        return levelIncomplete && currentDifficulty.ListOfLevelCompletedWords(currentGameLevelIndex).Count == dataController.gameData.allLevelData[currentGameLevelIndex].words.Length;
    }
    private bool CanUnlockNormalAndHardDifficulty()
    {
        return currentGameLevelIndex == DataController.LEVEL_TEN && SpeltAtleast51PercentOfLevelWords() && IsEasyDifficulty() && NormalDifficultyIsLocked();
    }
    private bool IsEasyDifficulty()
    {
        return DataController.DIFFICULTY_EASY.Equals(currentDifficulty.name);
    }
    private bool IsNormalDifficulty()
    {
        return DataController.DIFFICULTY_NORMAL.Equals(currentDifficulty.name);
    }
    private bool IsHardDifficulty()
    {
        return DataController.DIFFICULTY_HARD.Equals(currentDifficulty.name);
    }
    private bool NormalDifficultyIsLocked()
    {
        return !dataController.playerData.difficultyUnlocked[1];
    }
    private bool ListDoesNotContainWord(string word)
    {
        return !currentDifficulty.ListOfLevelCompletedWords(currentGameLevelIndex).Contains(word);
    }
    private int DetermineNumberOfEnemyShipsAllowed()
    {
        switch (currentGameLevelIndex)
        {
            case LEVEL_ONE:
            case LEVEL_TWO:
            case LEVEL_THREE:
            case LEVEL_FOUR:
            case LEVEL_FIVE:
                return 0;
            case LEVEL_SIX:
                return 1;
            case LEVEL_SEVEN:
            case LEVEL_EIGHT:
                return 2;
            case LEVEL_NINE:
            case LEVEL_TEN:
                return 3;
            default:
                return -1;
        }
    }
    private int DetermineNumberOfHazards()
    {
        switch (currentGameLevelIndex)
        {
            case LEVEL_ONE:
            case LEVEL_TWO:
                return 1;
            case LEVEL_THREE:
            case LEVEL_FOUR:
                return 2;
            case LEVEL_FIVE:
                return 3;
            case LEVEL_SIX:
            case LEVEL_SEVEN:
            case LEVEL_EIGHT:
            case LEVEL_NINE:
            case LEVEL_TEN:
               return 4;
            default:
                return -1;
        }
    }

    private bool MoreThanOneLetterLeft()
    {
        return targetLetterIndex + 1 <= (selectedWord.Length - 1);
    }
    private bool MoreThanTwoLettersLeft()
    {
        return targetLetterIndex + 2 <= (selectedWord.Length - 1);
    }
    private bool MoreThanThreeLettersLeft()
    {
        return targetLetterIndex + 3 <= (selectedWord.Length - 1);
    }
    private bool MoreThanFourLettersLeft()
    {
        return targetLetterIndex + 4 <= (selectedWord.Length - 1);
    }

    private bool BetweenNumbers(int num, int num1, int num2)
    {
        return num > num1 && num <= num2;
    }
    private GameObject PickALetterBlock(int index)
    {
        char[] letters = selectedWord.ToCharArray();
        int letterAscii = ConvertCharacterToAscii(letters[index]);
        int letterIndex = letterAscii - LETTER_A_ASCII;
        return blocks[letterIndex];
    }
}