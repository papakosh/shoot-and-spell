using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject[] pickupHelpMessages;
    public GameObject[] onScreenMessagesDisplay;
    public GameObject RoundBeginMessage;
    public GameObject RoundOverMessage;

    public GameObject[] hazards;
    public GameObject[] blocks;
    public GameObject[] pickups;
    public Vector3 debrisOrigin;
    public int debrisCount;
    public float debrisSpawnWait, debrisStartWait, debrisWaveWait;
    public static GameController instance = null;
    public GameObject gameBackground;
    [HideInInspector]
    public bool isDead;
    [HideInInspector]
    public bool isGamePaused;
    public float startCountdown;

    public Slider xpBar;
    public Slider hpBar;
    public Text rankText;
    public GameObject xpAddedText;
    public GameObject hpChangedText;
    public GameObject dualShotStatusIcon;
    public GameObject armorStatusIcon;
    public GameObject teleportStatusIcon;
    public GameObject joystickControlLeft;
    public GameObject joystickControlRight;

    public GameObject playerShip;
    public float hitFlashWait = 0.125f;
    public float slowDownFlashWait = 0.25f;
    public int numberOfFlashes = 3;

    public GameObject[] panelOfPossibleLetters;
    public GameObject[] selectedWordPanel;
    public string selectedWord;
    public GameObject mainMenuButton;

    private int gameLevel;
    private Color letterMatchedColor = new Color32(212, 175, 55, 255);
    private string[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K", "L", "M", "N", "O", "P", "Q", "R","S", "T", "U", "V", "W", "X", "Y", "Z"
        };

    private DataController dataController;
    private Difficulty currentDifficulty;
    private AudioSource _audio;
    private bool gameOver;
    private bool nextLevelUnlocked = false;
    private bool normalHardDifficultyUnlocked = false;
    private bool nextRankAchieved = false;
    private int levelCompleteBonus;
    private bool levelIncomplete;
    private bool hasCompletedLevel = false;
    private int enemyShipsAllowed;

    private GameObject[] debrisArray;

    private int targetLetterIndex;
    private int[] indicesForLettersFromSelectedWord;
    private AudioClip selectedWordClip;
    private int selectedWordIndex;

    private int playerRank;
    private int playerXP;
    private int xpAdded;
    private int levelXPModifier = 1;
    private Color normalColor = Color.white;
    private Color hitColor = Color.red;
    private Color bufferColor = Color.yellow;    
    private int playerStreak = 0;
    private const float streakModifier = 0.05f;
    private float rankBaseXP;
    private bool reachedMaxRank;   
  
    private string[] endOfRoundMsgs = {"x#", "LEVEL # UNLOCKED", "# RANK ACHIEVED", "NORMAL & HARD UNLOCKED", "LEVEL COMPLETED +# XP"};
    private string[] pickupHeaders = { "HEALTH", "DUAL SHOT", "ARMOR", "TELEPORT" };
    private string[] pickupMsgs = { "Add 1 point to HP", "Fire Two Bolts", "Absorb Any Damage", "Tap Anywhere And Move" };
    private string resumePlayingMessage = "Tap Screen to Resume";

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        dataController = FindObjectOfType<DataController>();
        currentDifficulty = dataController.currentDifficulty;

        _audio = GetComponent<AudioSource>();
        gameLevel = PlayerPrefs.GetInt("Level");
        selectedWord = RandomWord();
        selectedWordClip = Resources.Load<AudioClip>(dataController.gameData.allLevelData[gameLevel].words[selectedWordIndex].audioPath);

        playerXP = currentDifficulty.playerXP;
        playerRank = currentDifficulty.playerRank;
        PlayerController.instance.maxHealth = GetMaxHealth();

        if (PlayerPrefs.HasKey("PreviousRoundHealth")) PlayerController.instance.currentHealth = PlayerPrefs.GetFloat("PreviousRoundHealth");
        else PlayerController.instance.currentHealth = PlayerController.instance.maxHealth;

        if (PlayerPrefs.HasKey("PlayerStreak")) playerStreak = PlayerPrefs.GetInt("PlayerStreak");
        else playerStreak = 0;

        if (playerRank == DataController.MASTER_RANK) reachedMaxRank = true;
        List<string> completedLevelList = currentDifficulty.ListOfLevelCompletedWords(gameLevel);
        if (completedLevelList.Count < dataController.gameData.allLevelData[gameLevel].words.Length) levelIncomplete = true;

        levelCompleteBonus = dataController.gameData.allLevelData[gameLevel].completionBonus;
        rankBaseXP = dataController.gameData.baseXP;

        mainMenuButton.GetComponent<Button>().interactable = false;
        SetLevelBackground();

        Time.timeScale = 1f;
        _audio.clip = selectedWordClip;
       
        // adjust spawn wait for level difficulty
        if (gameLevel > 0)
            debrisSpawnWait = dataController.gameData.spawnWait - (dataController.gameData.spawnWaitDecrement * gameLevel);

        // adjust wave wait for level difficulty
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
    }
    private void SetLevelBackground()
    {
        Texture nebulaBackground = GetBackGroundTexture(gameLevel);
        GameObject backgroundChild = gameBackground.transform.GetChild(0).gameObject;
        backgroundChild.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
        gameBackground.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
    }
    private Texture GetBackGroundTexture(int gameLevel)
    {
        return Resources.Load<Texture>(dataController.gameData.allLevelData[gameLevel].backgroundPath);
    }
    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
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
        gameOver = false;
        isGamePaused = true;
        levelXPModifier = gameLevel + 1;
        RoundBeginMessage.SetActive(true);
        playerShip.SetActive(false);
        StartCoroutine(Countdown(startCountdown));
        targetLetterIndex = 0;
        StartCoroutine(SpawnWaves());
    }
    IEnumerator Countdown(float seconds)
    {
        float counter = seconds;
        while (counter >= 0)
        {
            yield return new WaitForSeconds(1);
            RoundBeginMessage.GetComponent<Text>().text = counter + "";
            counter--;

        }

        RoundBeginMessage.SetActive(false);
        playerShip.SetActive(true);
        isGamePaused = false;
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
                pickupHelpMessages[0].SetActive(false);
                pickupHelpMessages[1].SetActive(false);
                pickupHelpMessages[2].SetActive(false);
                pickupHelpMessages[3].SetActive(false);
                isGamePaused = false;
                Time.timeScale = 1f;
            }
        }
        if (!_audio.isPlaying)
            _audio.volume = DataController.DEFAULT_VOL;
    }
    public string GetRankText(int rank)
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
    public void Unpause()
    {
        Time.timeScale = 1f;
        RoundBeginMessage.SetActive(false);
        playerShip.SetActive(true);
        pickupHelpMessages[0].SetActive(false);
        pickupHelpMessages[1].SetActive(false);
        pickupHelpMessages[2].SetActive(false);
        pickupHelpMessages[3].SetActive(false);
        PlayerPrefs.SetInt("InRound", 1);
        _audio.clip = selectedWordClip;
        isGamePaused = false;
    }
    public IEnumerator HandleHealthText(int delay, float amt, bool damage)
    {

        hpChangedText.SetActive(true);
        if (damage)
        {
            hpChangedText.GetComponent<Text>().text = "-" + amt + " HP";
        }
        else
        {
            hpChangedText.GetComponent<Text>().text = "+" + amt + " HP";
        }
        hpChangedText.GetComponent<Text>().CrossFadeAlpha(0, 3.0f, false);
        yield return new WaitForSeconds(delay);
        hpChangedText.GetComponent<Text>().CrossFadeAlpha(1, 0.0f, false);
        hpChangedText.SetActive(false);
    }
    public void LevelUp()
    {
        PlayerController.instance.maxHealth = GetMaxHealth();
        PlayerController.instance.currentHealth = PlayerController.instance.maxHealth;
        PlayerPrefs.DeleteKey("PlayerHealth");
    }
    public void RoundLose()
    {
        gameOver = true;
        RoundOverMessage.GetComponent<Text>().text = "Better luck next time! Tap to continue.";
        RoundOverMessage.SetActive(true);
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
        PlayerPrefs.DeleteKey("DualShot");
        PlayerPrefs.DeleteKey("Armor");
        PlayerPrefs.DeleteKey("Teleport");
    }
    public void ContinuePlaying()
    {
        SceneManager.LoadScene("Game");
    }
    public void PlayWord()
    {
        _audio.clip = selectedWordClip;
        _audio.volume = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        _audio.Play();
    }
    public void GoHome()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
        PlayerPrefs.SetInt("InRound", 0);
        PlayerPrefs.DeleteKey("DualShot");
        PlayerPrefs.DeleteKey("Armor");
        PlayerPrefs.DeleteKey("Teleport");
        SceneManager.LoadScene("MainMenu");
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
                break;
            case 2:
                break;
            case 3:
                int pickupNum = 0;
                if (playerRank > DataController.CHIEF_RANK)
                    pickupNum = UnityEngine.Random.Range(0, 4);
                else if (playerRank > DataController.PILOT_RANK)
                    pickupNum = UnityEngine.Random.Range(0, 3);
                else if (playerRank > DataController.CADET_RANK)
                    pickupNum = UnityEngine.Random.Range(0, 2);

                bool firstPickup = IsFirstTimeForPickup(pickupNum);
                if (firstPickup)
                {
                    Instantiate(pickups[pickupNum], pickupTransform.position, rotateQuaternion);
                    // determine msg text
                    if (pickupNum == 0)
                    {
                        pickupHelpMessages[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_health");
                        pickupHelpMessages[1].GetComponent<Text>().text = pickupHeaders[0];
                        pickupHelpMessages[2].GetComponent<Text>().text = pickupMsgs[0];
                        pickupHelpMessages[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("HEALTHPICKUP", "YES");
                    }
                    else if (pickupNum == 1)
                    {
                        pickupHelpMessages[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_dual_shot");
                        pickupHelpMessages[1].GetComponent<Text>().text = pickupHeaders[1];
                        pickupHelpMessages[2].GetComponent<Text>().text = pickupMsgs[1];
                        pickupHelpMessages[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("DUALSHOTPICKUP", "YES");
                    }
                    else if (pickupNum == 2)
                    {
                        pickupHelpMessages[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_armor");
                        pickupHelpMessages[1].GetComponent<Text>().text = pickupHeaders[2];
                        pickupHelpMessages[2].GetComponent<Text>().text = pickupMsgs[2];
                        pickupHelpMessages[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("ARMORPICKUP", "YES");
                    }
                    else if (pickupNum == 3)
                    {
                        pickupHelpMessages[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pickups/icon_teleport");
                        pickupHelpMessages[1].GetComponent<Text>().text = pickupHeaders[3];
                        pickupHelpMessages[2].GetComponent<Text>().text = pickupMsgs[3];
                        pickupHelpMessages[3].GetComponent<Text>().text = resumePlayingMessage;
                        PlayerPrefs.SetString("TELEPORTPICKUP", "YES");
                    }

                    for(int i =0; i < pickupHelpMessages.Length; i++)
                    {
                        pickupHelpMessages[i].SetActive(true);
                    }
                    isGamePaused = true;
                    Time.timeScale = 0f;
                }
                else
                    Instantiate(pickups[pickupNum], pickupTransform.position, rotateQuaternion);
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            default:
                break;
        }
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
    private void RefreshUI()
    {
        rankText.text = GetRankText(playerRank);
        if (!reachedMaxRank)
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
    public void RefreshHealthBar(float amt, bool isDamaged)
    {
        StartCoroutine(HandleHealthText(3, amt, isDamaged));

        hpBar.maxValue = PlayerController.instance.maxHealth;
        hpBar.value = PlayerController.instance.currentHealth;
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
        double baseXP = rankBaseXP;
        return Math.Floor(baseXP * Math.Pow(rank + 1, exponent));
    }
    private void CheckProgression()
    {
        if (!reachedMaxRank)
        {
            int currentRankXP = (int)CalculateRankXP(playerRank);
            if (playerXP > currentRankXP)
            {
                playerRank++;
                playerXP = playerXP - currentRankXP;
                LevelUp();
                nextRankAchieved = true;

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
            nextLevelUnlocked = true;
        }else if (levelIncomplete && completedLevelList.Count == dataController.gameData.allLevelData[gameLevel].words.Length)
        {
            playerXP = playerXP + levelCompleteBonus;
            hasCompletedLevel = true;
            if (playerRank != DataController.MASTER_RANK)
            {
                int currentRankXP = (int)CalculateRankXP(playerRank);
                if (playerXP > currentRankXP)
                {
                    playerRank++;
                    playerXP = playerXP - currentRankXP;
                    LevelUp();
                    nextRankAchieved = true;

                }
                currentDifficulty.playerXP = playerXP;
                currentDifficulty.playerRank = playerRank;
                currentDifficulty.AddToListOfLevelCompletedWords(gameLevel, selectedWord);
                dataController.SavePlayerData();
            }
            else reachedMaxRank = true;
        }
        else
        {
            // On easy difficulty, just completed the last level and normal difficulty not unlocked, then unlock normal and hard difficulty
            if (gameLevel == 9  && dataController.playerData.difficultySelected.Equals(DataController.DIFFICULTY_EASY) && completedLevelList.Count >= ((dataController.gameData.allLevelData[gameLevel].words.Length / 2) + 1) && dataController.playerData.difficultyUnlocked[1] == false)
            {
                dataController.UnlockNormalAndHardDifficulty();
                normalHardDifficultyUnlocked = true;
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
        xpAdded = (int)xpEarned;
        playerXP = playerXP + xpAdded;
    }
   IEnumerator EndOfRoundStats()
    {
        if (!reachedMaxRank)
        {
            xpAddedText.SetActive(true);
            xpAddedText.GetComponent<Text>().text = "+" + xpAdded + " xp ";
            xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
        }

        onScreenMessagesDisplay[0].SetActive(true);
        onScreenMessagesDisplay[0].GetComponent<Image>().CrossFadeAlpha(0, 9.0f, true);
        float playerStreakCount = playerStreak / 0.05f;

        onScreenMessagesDisplay[1].GetComponent<Text>().text = endOfRoundMsgs[0].Replace("#",""+((int)playerStreakCount + 1));
        onScreenMessagesDisplay[1].SetActive(true);
        onScreenMessagesDisplay[1].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
        yield return new WaitForSeconds(0.5f);

        if (nextRankAchieved && !nextLevelUnlocked && !normalHardDifficultyUnlocked && !hasCompletedLevel)
        {
            onScreenMessagesDisplay[2].SetActive(true);
            onScreenMessagesDisplay[2].GetComponent<Text>().text = endOfRoundMsgs[2].Replace("#", "" + GetRankText(playerRank).ToUpper());
            onScreenMessagesDisplay[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            yield return new WaitForSeconds(0.5f);
        }else if (nextRankAchieved && (nextLevelUnlocked || normalHardDifficultyUnlocked || hasCompletedLevel))
        {
            onScreenMessagesDisplay[2].SetActive(true);
            onScreenMessagesDisplay[2].GetComponent<Text>().text = endOfRoundMsgs[2].Replace("#", GetRankText(playerRank).ToUpper());
            onScreenMessagesDisplay[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            if (nextLevelUnlocked)
            {
                onScreenMessagesDisplay[3].SetActive(true);
                onScreenMessagesDisplay[3].GetComponent<Text>().text = endOfRoundMsgs[1].Replace("#", "" + (gameLevel + 2));
                onScreenMessagesDisplay[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }
            else if (normalHardDifficultyUnlocked)
            {
                onScreenMessagesDisplay[3].SetActive(true);
                onScreenMessagesDisplay[3].GetComponent<Text>().text = endOfRoundMsgs[3];
                onScreenMessagesDisplay[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }else if (hasCompletedLevel)
            {
                onScreenMessagesDisplay[3].SetActive(true);

                onScreenMessagesDisplay[3].GetComponent<Text>().text = endOfRoundMsgs[4].Replace("#", ""+ levelCompleteBonus);
                onScreenMessagesDisplay[3].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);

                if (!reachedMaxRank)
                {
                    xpAddedText.SetActive(true);
                    xpAddedText.GetComponent<Text>().text = "+" + levelCompleteBonus + " xp ";
                    xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
                }
                hasCompletedLevel = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
        else if (nextLevelUnlocked || normalHardDifficultyUnlocked || hasCompletedLevel)
        {
            if (nextLevelUnlocked)
            {
                onScreenMessagesDisplay[2].SetActive(true);
                onScreenMessagesDisplay[2].GetComponent<Text>().text = endOfRoundMsgs[1].Replace("#", "" + (gameLevel + 2));
                onScreenMessagesDisplay[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }
            else if (normalHardDifficultyUnlocked)
            {
                onScreenMessagesDisplay[2].SetActive(true);
                onScreenMessagesDisplay[2].GetComponent<Text>().text = endOfRoundMsgs[3];
                onScreenMessagesDisplay[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
            }else if (hasCompletedLevel)
            {
                onScreenMessagesDisplay[2].SetActive(true);
                onScreenMessagesDisplay[2].GetComponent<Text>().text = endOfRoundMsgs[4].Replace("#", "" + levelCompleteBonus);
                onScreenMessagesDisplay[2].GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
                if (!reachedMaxRank)
                {
                    xpAddedText.SetActive(true);
                    xpAddedText.GetComponent<Text>().text = "+" + levelCompleteBonus + " xp ";
                    xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 9.0f, true);
                }
                hasCompletedLevel = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void RoundWin()
    {
        CalculateWordScore();
        CheckProgression();
        gameOver = true;
        RoundOverMessage.GetComponent<Text>().text = "Good job! Tap to continue.";
        RoundOverMessage.SetActive(true);
        playerShip.SetActive(false);
        StartCoroutine(EndOfRoundStats());

        PlayerPrefs.SetFloat("PlayerStreak", playerStreak + streakModifier);
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
        GameObject[] blocksArray = null;

        if (targetLetterIndex < 9)
            blocksArray = new GameObject[9];

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
   public IEnumerator BeenHit()
    {
        if (playerShip != null && playerShip.activeSelf)
        {
            for (int i = 1; i <= numberOfFlashes; i++)
            {
                if (playerShip != null && playerShip.activeSelf)
                {
                    playerShip.GetComponent<Renderer>().material.color = hitColor;
                    yield return new WaitForSeconds(hitFlashWait);
                }
                if (playerShip != null && playerShip.activeSelf)
                {
                    playerShip.GetComponent<Renderer>().material.color = normalColor;
                    yield return new WaitForSeconds(hitFlashWait);
                }
            }
        }
    }
    public IEnumerator ArmorActivated()
    {
        for (int i = 1; i <= numberOfFlashes; i++)
        {
            playerShip.GetComponent<Renderer>().material.color = bufferColor;
            yield return new WaitForSeconds(hitFlashWait);
            playerShip.GetComponent<Renderer>().material.color = normalColor;
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
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(debrisStartWait + startCountdown);
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
    IEnumerator DisplayWord(float delay)
    {
        if (!currentDifficulty.name.Equals(DataController.DIFFICULTY_HARD))
        {
            indicesForLettersFromSelectedWord = CalculateTargetIndices();

            for (int i = 0; i < selectedWordPanel.Length; i++)
            {
                if (i < indicesForLettersFromSelectedWord.Length)
                {
                    selectedWordPanel[i].GetComponent<Image>().sprite = panelOfPossibleLetters[indicesForLettersFromSelectedWord[i]].GetComponent<Image>().sprite;
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
                            selectedWordPanel[i].GetComponent<Image>().sprite = panelOfPossibleLetters[indicesForLettersFromSelectedWord[i]].GetComponent<Image>().sprite;
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
                    selectedWordPanel[i].GetComponent<Image>().sprite = panelOfPossibleLetters[indicesForLettersFromSelectedWord[i]].GetComponent<Image>().sprite;
                    selectedWordPanel[i].SetActive(false);
                }
                else selectedWordPanel[i].SetActive(false);
            }
        }
    }
    private void OnDestroy()
    {
        if (!isDead) PlayerPrefs.SetFloat("PreviousRoundHealth", PlayerController.instance.currentHealth);
    }
}