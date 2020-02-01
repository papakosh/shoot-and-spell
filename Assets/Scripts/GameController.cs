using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Description: Manages the user experience as well as most of the gameplay, including endless runner of asteroids, letters, and enemy ships; scoring XP for words 
 * spelled; earning player rank; spawning pickups; and finally, game difficulty.
 * 
 * Details - 
 * LoadMainMenu - Delete temporary data, unpause the game, and finally load the main menu.
 * PlayWord - Play the current word
 * ProcessHit - Evaluate the letter that was hit for the next letter in the current word. For non-matches, slow down the player's ship. 
 * For matches, if last letter, end round; else, increment to next letter.
 * SpawnRandomPickup - Pick a random number between 1 and 6. When the number 3 is picked, choose a random pickup depending on the player's rank and spawn. 
 * The first time a pickup appears,the game pauses and a usage tip displays to the user.
 * ResumeGame - Hide pickup messages, show the player ship and unpause the game
 * LoseRound - Show a concillatory message, delete temporary data, and mark round as over.
 * PlayAnotherRound - Reload the game on the current level
 * RefreshHealthBar - Show message of +HP when adding health to the player or -HP when subtracting health from the player, and then update hp bar to reflect 
 * the new value
 * PlayerShipHit - The ship's color flashes for several seconds, alternating between red and white, to indicate damage taken after hit
 * ArmorActive - The ship's color flashes for several seconds, alternating between yellow and white, to indicate no damage taken after hit
 * UpdateTeleportSatusIcon - Change status icon on UI to reflect whether player has teleport skill or not
 * UpdateArmorStatusIcon - Change status icon on UI to reflect whether player has armor skill or not
 * UpdateDualShotStatusIcon - Change status icon on UI to reflect whether player has dual shot skill or not
 * Awake - Instantiate instance of gamecontroller class, locate the data controller object, set the current difficulty, and initialize audio source object
 * Start - Setup the game level (initial level values, background image, debris wait times, word to be spelled), Setup the player data (?), Setup UI (?), 
 * Coutdown from 5 seconds, and finally spawn waves of debris (asteroids, enemy ships, and letter blocks)
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
    public float hitFlashWait = 0.125f;
    public float slowDownFlashWait = 0.25f;
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

    private int gameLevel;
    private bool roundOver;
    private int levelXPModifier = 1;
    private int levelCompleteBonus;
    private bool levelIncomplete;
    private GameObject[] debrisArray;
    private Color letterMatchedColor = new Color32(212, 175, 55, 255);
    private int targetLetterIndex; 
    private AudioClip selectedWordClip;
    private int selectedWordIndex;
    private string[] endOfRoundMsgs = { "x#", "LEVEL # UNLOCKED", "# RANK ACHIEVED", "NORMAL & HARD UNLOCKED", "LEVEL COMPLETED +# XP" };
    private string[] pickupHeaders = { "HEALTH", "DUAL SHOT", "ARMOR", "TELEPORT" };
    private string[] pickupMsgs = { "Add 1 point to HP", "Fire Two Bolts", "Absorb Any Damage", "Tap Anywhere And Move" };
    private string resumePlayingMessage = "Tap Screen to Resume";
    
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
    private const string HEALTH_PICKUP_PATH = "Sprites/Pickups/health_icon";
    private const string DUALSHOT_PICKUP_PATH = "Sprites/Pickups/dual_shot_icon";
    private const string ARMOR_PICKUP_PATH = "Sprites/Pickups/armor_icon";
    private const string TELEPORT_PICKUP_PATH = "Sprites/Pickups/teleport_icon";

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

    private string selectedWord;
    private int playerRank;
    private int playerXP;
    private int xpPlayerEarned;
    private bool playerUnlockedNextLevel = false;
    private bool playerUnlockedNormalHardDifficulty = false;
    private bool playerAchievedNextRank = false;
    private bool hasPlayerCompletedLevel = false;
    private Color shipNormalColor = Color.white;
    private Color shipHitColor = Color.red;
    private Color shipAbsorbedDamageColor = Color.yellow;
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
    public void ProcessHit(string hitLetter)
    {
        string targetLetter;
        if (IsLastLetter()) targetLetter = selectedWord.Substring(targetLetterIndex);
        else targetLetter = selectedWord.Substring(targetLetterIndex, 1);

        if (LettersMatch(targetLetter, hitLetter))
        {
            selectedWordPanel[targetLetterIndex].SetActive(true);
            selectedWordPanel[targetLetterIndex].GetComponent<Image>().color = letterMatchedColor;

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
                        if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(EASY_SEEN_HEALTH_PICKUP_KEY, "YES");
                        else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(EASY_SEEN_DUALSHOT_PICKUP_KEY, "YES");
                        else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(EASY_SEEN_ARMOR_PICKUP_KEY, "YES");
                        else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(EASY_SEEN_TELEPORT_PICKUP_KEY, "YES");
                    }else if (IsNormalDifficulty())
                    {
                        if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_HEALTH_PICKUP_KEY, "YES");
                        else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_DUALSHOT_PICKUP_KEY, "YES");
                        else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_ARMOR_PICKUP_KEY, "YES");
                        else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(NORMAL_SEEN_TELEPORT_PICKUP_KEY, "YES");
                    }
                    else if (IsHardDifficulty())
                    {
                        if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(HARD_SEEN_HEALTH_PICKUP_KEY, "YES");
                        else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(HARD_SEEN_DUALSHOT_PICKUP_KEY, "YES");
                        else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(HARD_SEEN_ARMOR_PICKUP_KEY, "YES");
                        else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(HARD_SEEN_TELEPORT_PICKUP_KEY, "YES");
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
                playerShip.GetComponent<Renderer>().material.color = shipHitColor;
                yield return new WaitForSeconds(hitFlashWait);
            }
            if (IsPlayerShipActive())
            {
                playerShip.GetComponent<Renderer>().material.color = shipNormalColor;
                yield return new WaitForSeconds(hitFlashWait);
            }
        }
    }
    public IEnumerator ArmorActive()
    {
        for (int i = 1; i <= numberOfFlashes; i++)
        {
            playerShip.GetComponent<Renderer>().material.color = shipAbsorbedDamageColor;
            yield return new WaitForSeconds(hitFlashWait);
            playerShip.GetComponent<Renderer>().material.color = shipNormalColor;
            yield return new WaitForSeconds(hitFlashWait);
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
        gameLevel = PlayerPrefs.GetInt(DataController.GAME_LEVEL_KEY);
        roundOver = false;
        levelXPModifier = gameLevel + 1;
        levelCompleteBonus = dataController.gameData.allLevelData[gameLevel].completionBonus;

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
            displayRoundMessages[5].GetComponent<Text>().text = counter + "";
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
                yield return new WaitForSeconds(slowDownFlashWait);
            }

            if (IsPlayerShipActive())
            {
                playerShip.transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(slowDownFlashWait);
            }
        }
        PlayerController.instance.NormalizeSpeed();
    }
    private string GetRankText(int rank)
    {
        switch (rank)
        {
            case DataController.RECRUIT_RANK:
                return "Space Recruit";
            case DataController.CADET_RANK:
                return "Space Cadet";
            case DataController.PILOT_RANK:
                return "Space Pilot";
            case DataController.ACE_RANK:
                return "Space Ace";
            case DataController.CHIEF_RANK:
                return "Space Chief";
            case DataController.CAPTAIN_RANK:
                return "Space Captain";
            case DataController.COMMANDER_RANK:
                return "Space Commander";
            case DataController.MASTER_RANK:
                return "Space Master";
            default:
                return "Undefined";
        }
    }
    private IEnumerator DisplayHPChangeText(int delay, float amt, bool isDamaged)
    {
        ShowHPChangeText();

        if (isDamaged) hpChangeText.GetComponent<Text>().text = "-" + amt + " HP";
        else hpChangeText.GetComponent<Text>().text = "+" + amt + " HP";

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
        currentDifficulty.AddToListOfLevelCompletedWords(gameLevel, selectedWord);
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
            currentDifficulty.levelsUnlocked[gameLevel + 1] = true;
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
        WordData[] words = dataController.gameData.allLevelData[gameLevel].words;
        string wordChosen = "";
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
            if (playerUnlockedNextLevel) ShowUnlockedLevelMessage();
            else if (playerUnlockedNormalHardDifficulty) ShowUnlockedDifficultyMessage();
            else if (hasPlayerCompletedLevel)ShowLevelCompleteMessage();
            yield return new WaitForSeconds(0.5f);
        }
        else if (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel)
        {
            if (playerUnlockedNextLevel) ShowUnlockedLevelMessage2();
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
                    if (selectedWordPanel[i].GetComponent<Image>().color != letterMatchedColor) selectedWordPanel[i].SetActive(false); 
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
        displayRoundMessages[6].GetComponent<Text>().text = "Better luck next time! Tap to continue.";
        displayRoundMessages[6].SetActive(true);
    }
    private void ShowRoundWonMessage()
    {
        displayRoundMessages[6].GetComponent<Text>().text = "Good job! Tap to continue.";
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
                return Resources.Load<Sprite>(HEALTH_PICKUP_PATH);
            case DUALSHOT_PICKUP:
                return Resources.Load<Sprite>(DUALSHOT_PICKUP_PATH);
            case ARMOR_PICKUP:
                return Resources.Load<Sprite>(ARMOR_PICKUP_PATH);
            case TELEPORT_PICKUP:
                return Resources.Load<Sprite>(TELEPORT_PICKUP_PATH);
            default: return null;
        }
    }

    private bool IsPlayerShipActive()
    {
        return playerShip != null && playerShip.activeSelf;
    }
    private Sprite GetSkillStatusSprite(string status)
    {
        return ACTIVE_STATUS.Equals(status) ? Resources.Load<Sprite>("Sprites/UI/Game/panel_active") : Resources.Load<Sprite>("Sprites/UI/Game/panel");
    }
    private Sprite GetMainMenuStatusSprite(string status)
    {
        return ACTIVE_STATUS.Equals(status) ? Resources.Load<Sprite>("Sprites/UI/Game/main_menu_active") : Resources.Load<Sprite>("Sprites/UI/Game/main_menu");
    }
    private void ShowCountdownMessage()
    {
        displayRoundMessages[5].SetActive(true);
    }
    private void HideCountdownMessage()
    {
        displayRoundMessages[5].SetActive(false);
    }
    private Texture GetBackGroundTexture(int gameLevel)
    {
        return Resources.Load<Texture>(dataController.gameData.allLevelData[gameLevel].backgroundPath);
    }
    private void CustomizeLevelBackground()
    {
        Texture nebulaBackground = GetBackGroundTexture(gameLevel);
        gameBackground.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
        GameObject backgroundChild = gameBackground.transform.GetChild(0).gameObject;
        backgroundChild.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
    }
    private void SetupWordSelected()
    {
        targetLetterIndex = 0;
        selectedWord = RandomWord();
        selectedWordClip = Resources.Load<AudioClip>(dataController.gameData.allLevelData[gameLevel].words[selectedWordIndex].audioPath);
        _audio.clip = selectedWordClip;
    }

    private bool AllWordsSpelt()
    {
        return currentDifficulty.ListOfLevelCompletedWords(gameLevel).Count == dataController.gameData.allLevelData[gameLevel].words.Length;
    }
    private void CustomizeLevelDebrisWaitTimes()
    {
        debrisSpawnWait = dataController.gameData.spawnWait - (dataController.gameData.spawnWaitDecrement * (gameLevel + 1));
        switch (gameLevel)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                debrisWaveWait = dataController.gameData.waveWait - 0;
                break;
            case 7:
            case 8:
                debrisWaveWait = dataController.gameData.waveWait - 1;
                break;
            case 9:
                debrisWaveWait = dataController.gameData.waveWait - 2;
                break;
        }
    }
    private void ShowPlayerStreakMessage()
    {
        displayRoundMessages[0].SetActive(true);
        displayRoundMessages[0].GetComponent<Image>().CrossFadeAlpha(0, 9.0f, true);

        float playerStreakCount = playerStreak / 0.05f;
        displayRoundMessages[1].GetComponent<Text>().text = endOfRoundMsgs[0].Replace("#", "" + ((int)playerStreakCount + 1));
        displayRoundMessages[1].SetActive(true);
        displayRoundMessages[1].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowRankAchievedMessage()
    {
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[2].Replace("#", "" + GetRankText(playerRank).ToUpper());
        displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowUnlockedLevelMessage()
    {
        displayRoundMessages[3].SetActive(true);
        displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[1].Replace("#", "" + (gameLevel + 2));
        displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }

    private void ShowUnlockedLevelMessage2()
    {
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[1].Replace("#", "" + (gameLevel + 2));
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
        displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[4].Replace("#", "" + levelCompleteBonus);
        displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowLevelCompleteMessage2()
    {
        displayRoundMessages[2].SetActive(true);
        displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[4].Replace("#", "" + levelCompleteBonus);
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
        return gameLevel < 9 && NextLevelIsLocked() && SpeltAtleast51PercentOfLevelWords();
    }
    private bool NextLevelIsLocked()
    {
        return !currentDifficulty.levelsUnlocked[gameLevel + 1];
    }
    private bool SpeltAtleast51PercentOfLevelWords()
    {
        return currentDifficulty.ListOfLevelCompletedWords(gameLevel).Count >= ((dataController.gameData.allLevelData[gameLevel].words.Length / 2) + 1);
    }
    private bool HasSpeltAllLevelWords()
    {
        return levelIncomplete && currentDifficulty.ListOfLevelCompletedWords(gameLevel).Count == dataController.gameData.allLevelData[gameLevel].words.Length;
    }
    private bool CanUnlockNormalAndHardDifficulty()
    {
        return gameLevel == 9 && SpeltAtleast51PercentOfLevelWords() && IsEasyDifficulty() && NormalDifficultyIsLocked();
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
        return !currentDifficulty.ListOfLevelCompletedWords(gameLevel).Contains(word);
    }
    private int DetermineNumberOfEnemyShipsAllowed()
    {
        switch (gameLevel)
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
        switch (gameLevel)
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