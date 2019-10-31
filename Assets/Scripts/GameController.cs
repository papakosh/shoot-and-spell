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

    private DataController dataController;
    private Difficulty currentDifficulty;
    private AudioSource _audio;

    private int gameLevel;
    private bool gameOver;
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
    private int playerStreak = 0;
    private const float playerStreakAdditive = 0.05f;
    private bool playerReachedMaxRank;

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        displayRoundMessages[1].SetActive(false);
        playerShip.SetActive(true);
        displayPickupMessage[0].SetActive(false);
        displayPickupMessage[1].SetActive(false);
        displayPickupMessage[2].SetActive(false);
        displayPickupMessage[3].SetActive(false);
        PlayerPrefs.SetInt("InRound", 1);
        _audio.clip = selectedWordClip;
        isGamePaused = false;
    }
    public void LoadMainMenu()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
        PlayerPrefs.SetInt("InRound", 0);
        PlayerPrefs.DeleteKey("DualShot");
        PlayerPrefs.DeleteKey("Armor");
        PlayerPrefs.DeleteKey("Teleport");
        SceneManager.LoadScene("MainMenu");
    }
    public void RoundLose()
    {
        gameOver = true;
        displayRoundMessages[6].GetComponent<Text>().text = "Better luck next time! Tap to continue.";
        displayRoundMessages[6].SetActive(true);
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
        PlayerPrefs.DeleteKey("DualShot");
        PlayerPrefs.DeleteKey("Armor");
        PlayerPrefs.DeleteKey("Teleport");
    }
    public void PlayAnotherRound()
    {
        SceneManager.LoadScene("Game");
    }
    public void PlayWord()
    {
        _audio.clip = selectedWordClip;
        _audio.volume = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        _audio.Play();
    }
    public bool ProcessHit(string hitLetter)
    {
        bool goodHit = false;
        if (targetLetterIndex < selectedWord.Length)
        {
            string targetLetter = "";
            if (targetLetterIndex < selectedWord.Length - 1)
                targetLetter = selectedWord.Substring(targetLetterIndex, 1);
            else
                targetLetter = selectedWord.Substring(targetLetterIndex);

            if (targetLetter.Equals(hitLetter))
            {
                goodHit = true;

                //mark complete
                GameObject[] targetPanel = GetTargetPanel();
                int targetPanelIndex = CalculateTargetPanelIndex();
                int elementIndex = 0;
                elementIndex = targetLetterIndex - (targetPanel.Length * targetPanelIndex);
                targetPanel[elementIndex].SetActive(true);
                targetPanel[elementIndex].GetComponent<Image>().color = letterMatchedColor;

                if (targetLetterIndex == selectedWord.Length - 1)
                {
                    RoundWin();
                }
                else
                {
                    targetLetterIndex++;
                }
            }
            else
            {
                StartCoroutine(PlayerController.instance.DecreaseSpeed());
                StartCoroutine(SlowDownEffect());
            }
        }
        return goodHit;
    }
    public void SpawnRandomPickup(Transform pickupTransform)
    {
        int num = UnityEngine.Random.Range(1, 6);
        Quaternion rotateQuaternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        switch (num)
        {
            case 1:
            case 2:
                break;
            case 3:
                int pickupNum = 0;
                if (playerRank > DataController.CHIEF_RANK) pickupNum = UnityEngine.Random.Range(0, 4);
                else if (playerRank > DataController.PILOT_RANK) pickupNum = UnityEngine.Random.Range(0, 3);
                else if (playerRank > DataController.CADET_RANK) pickupNum = UnityEngine.Random.Range(0, 2);

                if (IsFirstTimeForPickup(pickupNum))
                {
                    Instantiate(pickups[pickupNum], pickupTransform.position, rotateQuaternion);

                    if (pickupNum == 0)
                    {
                        displayPickupMessage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_health");
                        displayPickupMessage[1].GetComponent<Text>().text = pickupHeaders[0];
                        displayPickupMessage[2].GetComponent<Text>().text = pickupMsgs[0];
                        displayPickupMessage[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("HEALTHPICKUP", "YES");
                    }
                    else if (pickupNum == 1)
                    {
                        displayPickupMessage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_dual_shot");
                        displayPickupMessage[1].GetComponent<Text>().text = pickupHeaders[1];
                        displayPickupMessage[2].GetComponent<Text>().text = pickupMsgs[1];
                        displayPickupMessage[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("DUALSHOTPICKUP", "YES");
                    }
                    else if (pickupNum == 2)
                    {
                        displayPickupMessage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_armor");
                        displayPickupMessage[1].GetComponent<Text>().text = pickupHeaders[2];
                        displayPickupMessage[2].GetComponent<Text>().text = pickupMsgs[2];
                        displayPickupMessage[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("ARMORPICKUP", "YES");
                    }
                    else if (pickupNum == 3)
                    {
                        displayPickupMessage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_teleport");
                        displayPickupMessage[1].GetComponent<Text>().text = pickupHeaders[3];
                        displayPickupMessage[2].GetComponent<Text>().text = pickupMsgs[3];
                        displayPickupMessage[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("TELEPORTPICKUP", "YES");
                    }

                    for (int i = 0; i < displayPickupMessage.Length; i++)
                    {
                        displayPickupMessage[i].SetActive(true);
                    }
                    isGamePaused = true;
                    Time.timeScale = 0f;
                }
                else
                    Instantiate(pickups[pickupNum], pickupTransform.position, rotateQuaternion);
                break;
            case 4:
            case 5:
            case 6: 
                break;
            default: break;
        }
    }
    public void RefreshHealthBar(float amt, bool isDamaged)
    {
        StartCoroutine(DisplayHPChangeText(3, amt, isDamaged));

        hpBar.maxValue = PlayerController.instance.maxHealth;
        hpBar.value = PlayerController.instance.currentHealth;
    }
    public IEnumerator BeenHit()
    {
        if (playerShip != null && playerShip.activeSelf)
        {
            for (int i = 1; i <= numberOfFlashes; i++)
            {
                if (playerShip != null && playerShip.activeSelf)
                {
                    playerShip.GetComponent<Renderer>().material.color = shipHitColor;
                    yield return new WaitForSeconds(hitFlashWait);
                }
                if (playerShip != null && playerShip.activeSelf)
                {
                    playerShip.GetComponent<Renderer>().material.color = shipNormalColor;
                    yield return new WaitForSeconds(hitFlashWait);
                }
            }
        }
    }
    public IEnumerator ArmorActivated()
    {
        for (int i = 1; i <= numberOfFlashes; i++)
        {
            playerShip.GetComponent<Renderer>().material.color = shipAbsorbedDamageColor;
            yield return new WaitForSeconds(hitFlashWait);
            playerShip.GetComponent<Renderer>().material.color = shipNormalColor;
            yield return new WaitForSeconds(hitFlashWait);
        }
    }
    public IEnumerator SlowDownEffect()
    {
        for (int i = 1; i <= numberOfFlashes * 2; i++)
        {
            if (playerShip != null && playerShip.activeSelf)
            {
                playerShip.transform.GetChild(0).gameObject.SetActive(false);
                yield return new WaitForSeconds(slowDownFlashWait);
            }

            if (playerShip != null && playerShip.activeSelf)
            {
                playerShip.transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(slowDownFlashWait);
            }
        }
    }
    public void UpdateTeleportStatusIcon(string newStatus)
    {
        if ("ACTIVE".Equals(newStatus)) teleportStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        else teleportStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_deactive");
    }
    public void UpdateArmorStatusIcon(string newStatus)
    {
        if ("ACTIVE".Equals(newStatus)) armorStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        else armorStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_deactive");
    }
    public void UpdateDualShotStatusIcon(string newStatus)
    {
        if ("ACTIVE".Equals(newStatus)) dualShotStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        else dualShotStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_deactive");
    }
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        dataController = FindObjectOfType<DataController>();
        currentDifficulty = dataController.currentDifficulty;
        _audio = GetComponent<AudioSource>();

        Time.timeScale = 1f;
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
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGamePaused)
            {
                isGamePaused = true;
                Time.timeScale = 0f;
                mainMenuButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_hangar_active");
                mainMenuButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                mainMenuButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_hangar");
                mainMenuButton.GetComponent<Button>().interactable = false;
                displayPickupMessage[0].SetActive(false);
                displayPickupMessage[1].SetActive(false);
                displayPickupMessage[2].SetActive(false);
                displayPickupMessage[3].SetActive(false);
                isGamePaused = false;
                Time.timeScale = 1f;
            }
        }
        if (!_audio.isPlaying)
            _audio.volume = DataController.DEFAULT_VOL;
    }
    private void SetupGameLevel()
    {
        gameLevel = PlayerPrefs.GetInt("Level");
        gameOver = false;
        isGamePaused = true;
        levelXPModifier = gameLevel + 1;

        Texture nebulaBackground = GetBackGroundTexture(gameLevel);
        gameBackground.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
        GameObject backgroundChild = gameBackground.transform.GetChild(0).gameObject;
        backgroundChild.GetComponent<Renderer>().material.mainTexture = nebulaBackground;

        selectedWord = RandomWord();
        selectedWordClip = Resources.Load<AudioClip>(dataController.gameData.allLevelData[gameLevel].words[selectedWordIndex].audioPath);
        _audio.clip = selectedWordClip;
        
        List<string> listOfLevelCompletedWords = currentDifficulty.ListOfLevelCompletedWords(gameLevel);
        if (listOfLevelCompletedWords.Count < dataController.gameData.allLevelData[gameLevel].words.Length) levelIncomplete = true;
        levelCompleteBonus = dataController.gameData.allLevelData[gameLevel].completionBonus;

        displayRoundMessages[5].SetActive(true);

        targetLetterIndex = 0;

        CustomizeLevelDebrisWaitTimes();
    }
    private Texture GetBackGroundTexture(int gameLevel)
    {
        return Resources.Load<Texture>(dataController.gameData.allLevelData[gameLevel].backgroundPath);
    }
    private void CustomizeLevelDebrisWaitTimes()
    {
        if (gameLevel > 0)
            debrisSpawnWait = dataController.gameData.spawnWait - (dataController.gameData.spawnWaitDecrement * gameLevel);
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
    private void SetupPlayerData()
    {
        playerXP = currentDifficulty.playerXP;
        playerRank = currentDifficulty.playerRank;
        PlayerController.instance.maxHealth = GetMaxHealth();

        if (PlayerPrefs.HasKey("PreviousRoundHealth")) PlayerController.instance.currentHealth = PlayerPrefs.GetFloat("PreviousRoundHealth");
        else PlayerController.instance.currentHealth = PlayerController.instance.maxHealth;

        if (PlayerPrefs.HasKey("PlayerStreak")) playerStreak = PlayerPrefs.GetInt("PlayerStreak");
        else playerStreak = 0;

        if (PlayerPrefs.HasKey("DualShot"))
        {
            PlayerController.instance.canFireDualShot = true;
            dualShotStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        }
        if (PlayerPrefs.HasKey("Armor"))
        {
            PlayerController.instance.canAbsorbDamage = true;
            armorStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        }
        if (PlayerPrefs.HasKey("Teleport"))
        {
            PlayerController.instance.canTeleport = true;
            teleportStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
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

        playerShip.SetActive(false);
    }
    private void SetupUI()
    {
        mainMenuButton.GetComponent<Button>().interactable = false;

        rankText.text = GetRankText(playerRank);
        if (!playerReachedMaxRank)
        {
            xpBar.maxValue = (int)CalculateRankXP(playerRank);
            xpBar.value = playerXP;
        }
        else
        {
            xpBar.maxValue = 1;
            xpBar.value = 1;
            xpAddedText.GetComponent<Text>().text = "MAXED";
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
        displayRoundMessages[5].SetActive(false);
        playerShip.SetActive(true);
        isGamePaused = false;
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
        PlayerPrefs.DeleteKey("PlayerHealth");
    }
    private bool IsFirstTimeForPickup(int number)
    {
        switch (number)
        {
            case 0: return !PlayerPrefs.HasKey("HEALTHPICKUP");
            case 1: return !PlayerPrefs.HasKey("DUALSHOTPICKUP");
            case 2: return !PlayerPrefs.HasKey("ARMORPICKUP");
            case 3: return !PlayerPrefs.HasKey("TELEPORTPICKUP");
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
    private double CalculateRankXP(int rank)
    {
        double exponent = dataController.gameData.xpModifier;
        double baseXP = dataController.gameData.baseXP;
        return Math.Floor(baseXP * Math.Pow(rank + 1, exponent));
    }
    private void CheckProgression()
    {
        if (!playerReachedMaxRank)
        {
            int currentRankXP = (int)CalculateRankXP(playerRank);
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
                int currentRankXP = (int)CalculateRankXP(playerRank);
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
        if (!playerReachedMaxRank)
        {
            xpAddedText.SetActive(true);
            xpAddedText.GetComponent<Text>().text = "+" + xpPlayerEarned + " xp ";
            xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
        }

        displayRoundMessages[0].SetActive(true);
        displayRoundMessages[0].GetComponent<Image>().CrossFadeAlpha(0, 9.0f, true);
        float playerStreakCount = playerStreak / 0.05f;

        displayRoundMessages[1].GetComponent<Text>().text = endOfRoundMsgs[0].Replace("#",""+((int)playerStreakCount + 1));
        displayRoundMessages[1].SetActive(true);
        displayRoundMessages[1].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
        yield return new WaitForSeconds(0.5f);

        if (playerAchievedNextRank && !playerUnlockedNextLevel && !playerUnlockedNormalHardDifficulty && !hasPlayerCompletedLevel)
        {
            displayRoundMessages[2].SetActive(true);
            displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[2].Replace("#", "" + GetRankText(playerRank).ToUpper());
            displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            yield return new WaitForSeconds(0.5f);
        }else if (playerAchievedNextRank && (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel))
        {
            displayRoundMessages[2].SetActive(true);
            displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[2].Replace("#", GetRankText(playerRank).ToUpper());
            displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            if (playerUnlockedNextLevel)
            {
                displayRoundMessages[3].SetActive(true);
                displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[1].Replace("#", "" + (gameLevel + 2));
                displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }
            else if (playerUnlockedNormalHardDifficulty)
            {
                displayRoundMessages[3].SetActive(true);
                displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[3];
                displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }else if (hasPlayerCompletedLevel)
            {
                displayRoundMessages[3].SetActive(true);
                displayRoundMessages[3].GetComponent<Text>().text = endOfRoundMsgs[4].Replace("#", ""+ levelCompleteBonus);
                displayRoundMessages[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);

                if (!playerReachedMaxRank)
                {
                    xpAddedText.SetActive(true);
                    xpAddedText.GetComponent<Text>().text = "+" + levelCompleteBonus + " xp ";
                    xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
                }
                hasPlayerCompletedLevel = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
        else if (playerUnlockedNextLevel || playerUnlockedNormalHardDifficulty || hasPlayerCompletedLevel)
        {
            if (playerUnlockedNextLevel)
            {
                displayRoundMessages[2].SetActive(true);
                displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[1].Replace("#", "" + (gameLevel + 2));
                displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }
            else if (playerUnlockedNormalHardDifficulty)
            {
                displayRoundMessages[2].SetActive(true);
                displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[3];
                displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }else if (hasPlayerCompletedLevel)
            {
                displayRoundMessages[2].SetActive(true);
                displayRoundMessages[2].GetComponent<Text>().text = endOfRoundMsgs[4].Replace("#", "" + levelCompleteBonus);
                displayRoundMessages[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
                if (!playerReachedMaxRank)
                {
                    xpAddedText.SetActive(true);
                    xpAddedText.GetComponent<Text>().text = "+" + levelCompleteBonus + " xp ";
                    xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
                }
                hasPlayerCompletedLevel = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void RoundWin()
    {
        CalculateWordScore();
        CheckProgression();
        gameOver = true;
        displayRoundMessages[6].GetComponent<Text>().text = "Good job! Tap to continue.";
        displayRoundMessages[6].SetActive(true);
        playerShip.SetActive(false);
        StartCoroutine(EndOfRoundStats());

        PlayerPrefs.SetFloat("PlayerStreak", playerStreak + playerStreakAdditive);
        if (PlayerController.instance.canFireDualShot) PlayerPrefs.SetInt("DualShot", 1);
        else PlayerPrefs.DeleteKey("DualShot");
        if (PlayerController.instance.canAbsorbDamage) PlayerPrefs.SetInt("Armor", 1);
        else PlayerPrefs.DeleteKey("Armor");
        if (PlayerController.instance.canTeleport) PlayerPrefs.SetInt("Teleport", 1);
        else PlayerPrefs.DeleteKey("Teleport");
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
    private int CalculateTargetPanelIndex()
    {
            return 0;
    }
    private GameObject[] GetTargetPanel()
    {
        return selectedWordPanel;
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

            if (gameOver) break;
        }
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
        if (!isPlayerDead) PlayerPrefs.SetFloat("PreviousRoundHealth", PlayerController.instance.currentHealth);
    }
}