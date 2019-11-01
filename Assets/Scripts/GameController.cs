using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public string selectedWord;
    public GameObject[] displayPickupMessage;
    public GameObject[] displayRoundMessages;

    [HideInInspector]
    public bool isPlayerDead;
    public GameObject playerShip;
    public Slider xpBar;
    public Slider hpBar;
    public Text rankText;
    public GameObject xpAddedText;
    public GameObject hpChangedText;
    public float hitFlashWait = 0.125f;
    public float slowDownFlashWait = 0.25f;
    public int numberOfFlashes = 3;
    public GameObject dualShotStatusIcon;
    public GameObject armorStatusIcon;
    public GameObject teleportStatusIcon;
    public GameObject joystickControlLeft;
    public GameObject joystickControlRight;
    public GameObject mainMenuButton;

    public const int HEALTH_PICKUP = 0;
    public const int DUALSHOT_PICKUP = 1;
    public const int ARMOR_PICKUP = 2;
    public const int TELEPORT_PICKUP = 3;
    public const string HEALTH_PICKUP_PATH = "Sprites/Pickups/icon_health";
    public const string DUALSHOT_PICKUP_PATH = "Sprites/Pickups/icon_dual_shot";
    public const string ARMOR_PICKUP_PATH = "Sprites/Pickups/icon_armor";
    public const string TELEPORT_PICKUP_PATH = "Sprites/Pickups/icon_teleport";
    public const string HEALTH_PICKUP_KEY = "SEEN_HEALTH";
    public const string DUALSHOT_PICKUP_KEY = "SEEN_DUALSHOT";
    public const string ARMOR_PICKUP_KEY = "SEEN_ARMOR";
    public const string TELEPORT_PICKUP_KEY = "SEEN_TELEPORT";
    public const string ACTIVE_STATUS = "ACTIVE";
    public const string INACTIVE_STATUS = "INACTIVE";
    public const string MAXIMUM_RANK_TEXT = "MAXED";

    private DataController dataController;
    private Difficulty currentDifficulty;
    private AudioSource _audio;

    private int gameLevel;
    private bool roundOver;
    private int enemyShipsAllowed;
    private int levelXPModifier = 1;
    private int levelCompleteBonus;
    private bool levelIncomplete;
    private GameObject[] debrisArray;
    private Color letterMatchedColor = new Color32(212, 175, 55, 255);
    private string[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K", "L", "M", "N", "O", "P", "Q", "R",
            "S", "T", "U", "V", "W", "X", "Y", "Z" };
    private int targetLetterIndex;
    private int[] indicesForLettersFromSelectedWord;
    private AudioClip selectedWordClip;
    private int selectedWordIndex;
    private string[] endOfRoundMsgs = { "x#", "LEVEL # UNLOCKED", "# RANK ACHIEVED", "NORMAL & HARD UNLOCKED", "LEVEL COMPLETED +# XP" };
    private string[] pickupHeaders = { "HEALTH", "DUAL SHOT", "ARMOR", "TELEPORT" };
    private string[] pickupMsgs = { "Add 1 point to HP", "Fire Two Bolts", "Absorb Any Damage", "Tap Anywhere And Move" };
    private string resumePlayingMessage = "Tap Screen to Resume";

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
    private const float playerStreakAdditive = 0.05f;
    private bool playerReachedMaxRank;

    public void LoadMainMenu()
    {
        PlayerPrefs.DeleteKey(DataController.PLAYER_HEALTH_KEY);
        PlayerPrefs.DeleteKey(DataController.PLAYER_STREAK_KEY);
        PlayerPrefs.DeleteKey(DataController.DUALSHOT_KEY);
        PlayerPrefs.DeleteKey(DataController.ARMOR_KEY);
        PlayerPrefs.DeleteKey(DataController.TELPORT_KEY);

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
        string targetLetter = "";
        if (IsLastLetter()) targetLetter = selectedWord.Substring(targetLetterIndex);
        else targetLetter = selectedWord.Substring(targetLetterIndex, 1);

        if (LettersMatch(targetLetter, hitLetter))
        {
            selectedWordPanel[targetLetterIndex].SetActive(true);
            selectedWordPanel[targetLetterIndex].GetComponent<Image>().color = letterMatchedColor;

            if (IsLastLetter()) RoundWin();
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

                    if (pickupChosen == HEALTH_PICKUP) PlayerPrefs.SetString(HEALTH_PICKUP_KEY, "YES");
                    else if (pickupChosen == DUALSHOT_PICKUP) PlayerPrefs.SetString(DUALSHOT_PICKUP_KEY, "YES");
                    else if (pickupChosen == ARMOR_PICKUP) PlayerPrefs.SetString(ARMOR_PICKUP_KEY, "YES");
                    else if (pickupChosen == TELEPORT_PICKUP) PlayerPrefs.SetString(TELEPORT_PICKUP_KEY, "YES");

                    displayPickupMessage[0].GetComponent<Image>().sprite = GetPickupSprite(pickupChosen);
                    displayPickupMessage[1].GetComponent<Text>().text = pickupHeaders[pickupChosen];
                    displayPickupMessage[2].GetComponent<Text>().text = pickupMsgs[pickupChosen];
                    displayPickupMessage[3].GetComponent<Text>().text = resumePlayingMessage;
                    ShowPickupMessage();

                    Time.timeScale = 0f;
                    isGamePaused = true;
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
        PlayerPrefs.DeleteKey(DataController.TELPORT_KEY);

        roundOver = true;
    }
    public void PlayAnotherRound()
    {
        SceneManager.LoadScene(DataController.GAME_SCENE);
    }
    public void RefreshHealthBar(float amt, bool isDamaged)
    {
        StartCoroutine(DisplayHPChangeText(3, amt, isDamaged));

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

        if (!AllWordsSpelt ()) levelIncomplete = true;

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
        if (PlayerPrefs.HasKey(DataController.TELPORT_KEY))
        {
            PlayerController.instance.canTeleport = true;
            teleportStatusIcon.GetComponent<Image>().sprite = GetSkillStatusSprite(ACTIVE_STATUS);
        }

        if (playerRank == DataController.MASTER_RANK) playerReachedMaxRank = true;

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
        if (!playerReachedMaxRank)
        {
            xpBar.maxValue = (int)CalculateXPForNextRank(playerRank);
            xpBar.value = playerXP;
        }
        else
        {
            xpBar.maxValue = 1;
            xpBar.value = 1;
            xpAddedText.GetComponent<Text>().text = MAXIMUM_RANK_TEXT;
            xpAddedText.SetActive(true);
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
    private IEnumerator DisplayHPChangeText(int delay, float amt, bool isDamage)
    {
        hpChangedText.SetActive(true);
        if (isDamage) hpChangedText.GetComponent<Text>().text = "-" + amt + " HP";
        else hpChangedText.GetComponent<Text>().text = "+" + amt + " HP";

        hpChangedText.GetComponent<Text>().CrossFadeAlpha(0, 3.0f, false);
        yield return new WaitForSeconds(delay);
        hpChangedText.GetComponent<Text>().CrossFadeAlpha(1, 0.0f, false);
        hpChangedText.SetActive(false);
    }
    private void LevelUp()
    {
        PlayerController.instance.maxHealth = GetMaxHealth();
        PlayerController.instance.currentHealth = PlayerController.instance.maxHealth;
        PlayerPrefs.DeleteKey(DataController.PLAYER_HEALTH_KEY);
    }
    private bool IsFirstTimeForPickup(int number)
    {
        switch (number)
        {
            case 0: return !PlayerPrefs.HasKey(HEALTH_PICKUP_KEY);
            case 1: return !PlayerPrefs.HasKey(DUALSHOT_PICKUP_KEY);
            case 2: return !PlayerPrefs.HasKey(ARMOR_PICKUP_KEY);
            case 3: return !PlayerPrefs.HasKey(TELEPORT_PICKUP_KEY);
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
            case DataController.CHIEF_RANK:  return 5.0f;
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
        if (!playerReachedMaxRank)
        {
            int currentRankXP = (int)CalculateXPForNextRank(playerRank);
            if (playerXP > currentRankXP)
            {
                playerRank++;
                playerXP = playerXP - currentRankXP;
                LevelUp();
                playerAchievedNextRank = true;
            }
            currentDifficulty.playerXP = playerXP;
            currentDifficulty.playerRank = playerRank;
            currentDifficulty.AddToListOfLevelCompletedWords(gameLevel, selectedWord);
            dataController.SavePlayerData();
        }
        else
        {
            currentDifficulty.playerXP = 0;
            currentDifficulty.playerRank = playerRank;
            currentDifficulty.AddToListOfLevelCompletedWords(gameLevel, selectedWord);
            dataController.SavePlayerData();
        }
        List<string> completedLevelList = dataController.currentDifficulty.ListOfLevelCompletedWords(gameLevel);
        if (gameLevel < 9 && !currentDifficulty.levelsUnlocked[gameLevel + 1] && completedLevelList.Count >= ((dataController.gameData.allLevelData[gameLevel].words.Length / 2) + 1)) 
            // at least 51% of the words spelled correctly, then mark complete
        {
            currentDifficulty.levelsUnlocked[gameLevel + 1] = true;
            dataController.SavePlayerData();
            playerUnlockedNextLevel = true;
        }else if (levelIncomplete && completedLevelList.Count == dataController.gameData.allLevelData[gameLevel].words.Length)
        {
            playerXP = playerXP + levelCompleteBonus;
            hasPlayerCompletedLevel = true;
            if (playerRank != DataController.MASTER_RANK)
            {
                int currentRankXP = (int)CalculateXPForNextRank(playerRank);
                if (playerXP > currentRankXP)
                {
                    playerRank++;
                    playerXP = playerXP - currentRankXP;
                    LevelUp();
                    playerAchievedNextRank = true;
                }
                currentDifficulty.playerXP = playerXP;
                currentDifficulty.playerRank = playerRank;
                currentDifficulty.AddToListOfLevelCompletedWords(gameLevel, selectedWord);
                dataController.SavePlayerData();
            }
            else playerReachedMaxRank = true;
        }
        else
        {
            // On easy difficulty, just completed the last level and normal difficulty not unlocked, then unlock normal and hard difficulty
            if (gameLevel == 9  && dataController.playerData.difficultySelected.Equals(DataController.DIFFICULTY_EASY) && completedLevelList.Count >= ((dataController.gameData.allLevelData[gameLevel].words.Length / 2) + 1) && dataController.playerData.difficultyUnlocked[1] == false)
            {
                dataController.UnlockNormalAndHardDifficulty();
                playerUnlockedNormalHardDifficulty = true;
            }
        }
    }
    private string RandomWord()
    {
        WordData[] words = dataController.gameData.allLevelData[gameLevel].words;
        selectedWordIndex = UnityEngine.Random.Range(0, words.Length);
        return words[selectedWordIndex].text;
    }
    private void CalculateWordScore()
    {
        double xpEarned = Math.Round (selectedWord.Length * (double)(levelXPModifier + playerStreak), MidpointRounding.AwayFromZero);
        xpPlayerEarned = (int)xpEarned;
        playerXP = playerXP + xpPlayerEarned;
    }
   private IEnumerator EndOfRoundStats()
    {
        if (!playerReachedMaxRank) ShowXPEarnedMessage();

        ShowPlayerStreakBackground();
        ShowPlayerStreakMessage();
        yield return new WaitForSeconds(0.5f);

        if (playerAchievedNextRank && !playerUnlockedNextLevel && !playerUnlockedNormalHardDifficulty && !hasPlayerCompletedLevel)
        {
            ShowRankAchievedMessage();
            yield return new WaitForSeconds(0.5f);
        }else if (playerAchievedNextRank && (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel))
        {
            ShowRankAchievedMessage();
            if (playerUnlockedNextLevel) ShowUnlockedLevelMessage();
            else if (playerUnlockedNormalHardDifficulty) ShowUnlockedDifficultyMessage();
            else if (hasPlayerCompletedLevel)
            {
                ShowLevelCompleteMessage();
                hasPlayerCompletedLevel = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
        else if (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel)
        {
            if (playerUnlockedNextLevel) ShowUnlockedLevelMessage2();
            else if (playerUnlockedNormalHardDifficulty) ShowUnlockedDifficultyMessage2();
            else if (hasPlayerCompletedLevel)
            {
                ShowLevelCompleteMessage2();
                hasPlayerCompletedLevel = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void RoundWin()
    {
        HidePlayerShip();
        ShowRoundWonMessage();
        CalculateWordScore();
        CheckProgression();
        StartCoroutine(EndOfRoundStats());

        PlayerPrefs.SetFloat(DataController.PLAYER_STREAK_KEY, playerStreak + playerStreakAdditive);
        if (PlayerController.instance.canFireDualShot) PlayerPrefs.SetInt(DataController.DUALSHOT_KEY, 1);
        else PlayerPrefs.DeleteKey(DataController.DUALSHOT_KEY);
        if (PlayerController.instance.canAbsorbDamage) PlayerPrefs.SetInt(DataController.ARMOR_KEY, 1);
        else PlayerPrefs.DeleteKey(DataController.ARMOR_KEY);
        if (PlayerController.instance.canTeleport) PlayerPrefs.SetInt(DataController.TELPORT_KEY, 1);
        else PlayerPrefs.DeleteKey(DataController.TELPORT_KEY);
        roundOver = true;
    }
    private void PopulateDebrisArray()
    {
        debrisArray = new GameObject[debrisCount];
        GameObject[] blocksArray = new GameObject[9];
        blocksArray[0] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex]];

        for (int j = 1; j < blocksArray.Length; j++)
        {
            int num = UnityEngine.Random.Range(0, 26);
            blocksArray[j] = blocks[num];
        }

        switch (gameLevel)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                enemyShipsAllowed = 0;
                break;
            case 5:
                enemyShipsAllowed = 1;
                break;
            case 6:
            case 7:
                enemyShipsAllowed = 2;
                break;
            case 8:
            case 9:
                enemyShipsAllowed = 3;
                break;

        }

        for (int i = 0; i < debrisCount; i++)
        {
            int random = UnityEngine.Random.Range(0, 3);
            if (random == 0)
            {
                int numHazards = 0;
                switch (gameLevel)
                {
                    case 0:
                    case 1:
                        numHazards = 1;
                        break;
                    case 2:
                    case 3:
                        numHazards = 2;
                        break;
                    case 4:
                        numHazards = 3;
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        numHazards = 4;
                        break;
                }

                int hazardChosen = UnityEngine.Random.Range(0, numHazards);
                if (hazardChosen == 3)
                {
                    if (enemyShipsAllowed > 0)
                    {
                        debrisArray[i] = hazards[hazardChosen];
                        enemyShipsAllowed--;
                    }
                    else debrisArray[i] = hazards[UnityEngine.Random.Range(0, 3)];
                }
                else debrisArray[i] = hazards[hazardChosen];
            }
            else
            {
                int num = UnityEngine.Random.Range(0, 100);
                if (num <= 44) debrisArray[i] = blocksArray[0]; // 45 % chance 
                else if (num > 44 && num <= 54) // 10 % chance
                {
                    if (targetLetterIndex + 1 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 1]];
                    else debrisArray[i] = blocksArray[1];
                }
                else if (num > 54 && num <= 64) // 10 % chance
                {
                    if (targetLetterIndex + 1 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 1]];
                    else debrisArray[i] = blocksArray[2];
                }
                else if (num > 64 && num <= 74) // 10 % chance
                {
                    if (targetLetterIndex + 2 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 2]];
                    else debrisArray[i] = blocksArray[3];
                }
                else if (num > 74 && num <= 79) // 5 % chance
                {
                    if (targetLetterIndex + 2 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 2]];
                    else debrisArray[i] = blocksArray[4];
                }
                else if (num > 79 && num <= 84) // 5 % chance
                {
                    if (targetLetterIndex + 3 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 3]];
                    else debrisArray[i] = blocksArray[5];
                }
                else if (num > 84 && num <= 89) // 5 % chance
                {
                    if (targetLetterIndex + 3 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 3]];
                    else debrisArray[i] = blocksArray[6];
                }
                else if (num > 89 && num <= 94) // 5 % chance
                {
                    if (targetLetterIndex + 4 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 4]];
                    else debrisArray[i] = blocksArray[7];
                }
                else if (num > 94) // 5 % chance
                {
                    if (targetLetterIndex + 4 <= indicesForLettersFromSelectedWord.Length - 1) debrisArray[i] = blocks[indicesForLettersFromSelectedWord[targetLetterIndex + 4]];
                    else debrisArray[i] = blocksArray[8];
                }
            }
        }
    }
    private int[] CalculateTargetIndices()
    {
        int[] targetIndices = new int[selectedWord.Length];
        string letter = selectedWord.Substring(0, 1);
        int letterIndex = 0;
        while (letterIndex < selectedWord.Length)
        {
            for (int j = 0; j < letters.Length; j++)
            {
                if (letter.Equals(letters[j]))
                {
                    targetIndices[letterIndex] = j;
                    letterIndex++;
                    if (letterIndex < selectedWord.Length - 1) letter = selectedWord.Substring(letterIndex, 1);
                    else letter = selectedWord.Substring(letterIndex);
                }
            }
        }
        return targetIndices;
    }
    private IEnumerator DisplayWord(float delay)
    {
        if (!currentDifficulty.name.Equals(DataController.DIFFICULTY_HARD))
        {
            indicesForLettersFromSelectedWord = CalculateTargetIndices();

            for (int i = 0; i < selectedWordPanel.Length; i++)
            {
                if (i < indicesForLettersFromSelectedWord.Length)
                {
                    selectedWordPanel[i].GetComponent<Image>().sprite = letterImages[indicesForLettersFromSelectedWord[i]].GetComponent<Image>().sprite;
                    selectedWordPanel[i].SetActive(true);
                }
                else selectedWordPanel[i].SetActive(false);
            }
            if (currentDifficulty.name.Equals(DataController.DIFFICULTY_NORMAL))
            {
                yield return new WaitForSeconds(delay);
                indicesForLettersFromSelectedWord = CalculateTargetIndices();

                for (int i = 0; i < selectedWordPanel.Length; i++)
                {
                    if (i < indicesForLettersFromSelectedWord.Length)
                    {
                        if (selectedWordPanel[i].GetComponent<Image>().color != letterMatchedColor)
                        {
                            selectedWordPanel[i].GetComponent<Image>().sprite = letterImages[indicesForLettersFromSelectedWord[i]].GetComponent<Image>().sprite;
                            selectedWordPanel[i].SetActive(false);
                        }
                    }
                    else
                    {
                        if (selectedWordPanel[i].GetComponent<Image>().color != letterMatchedColor) selectedWordPanel[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            indicesForLettersFromSelectedWord = CalculateTargetIndices();

            for (int i = 0; i < selectedWordPanel.Length; i++)
            {
                if (i < indicesForLettersFromSelectedWord.Length)
                {
                    selectedWordPanel[i].GetComponent<Image>().sprite = letterImages[indicesForLettersFromSelectedWord[i]].GetComponent<Image>().sprite;
                    selectedWordPanel[i].SetActive(false);
                }
                else selectedWordPanel[i].SetActive(false);
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
    private void ShowPickupMessage()
    {
        displayPickupMessage[0].SetActive(true);
        displayPickupMessage[1].SetActive(true);
        displayPickupMessage[2].SetActive(true);
        displayPickupMessage[3].SetActive(true);
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
    private Sprite GetSkillStatusSprite(String status)
    {
        return "ACTIVE".Equals(status) ? Resources.Load<Sprite>("Sprites/panel_active") : Resources.Load<Sprite>("Sprites/panel_deactive");
    }
    private Sprite GetMainMenuStatusSprite(String status)
    {
        return "ACTIVE".Equals(status) ? Resources.Load<Sprite>("Sprites/button_hangar_active") : Resources.Load<Sprite>("Sprites/button_hangar");
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
    private void ShowPlayerStreakBackground()
    {
        displayRoundMessages[0].SetActive(true);
        displayRoundMessages[0].GetComponent<Image>().CrossFadeAlpha(0, 9.0f, true);
    }
    private void ShowPlayerStreakMessage()
    {
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
        xpAddedText.SetActive(true);
        xpAddedText.GetComponent<Text>().text = "+" + xpPlayerEarned + " xp ";
        xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
    }
}