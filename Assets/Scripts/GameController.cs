﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject pickupHelpMessage;
    public GameObject[] hazards;
    public GameObject[] blocks;
    public Vector3 spawnValues;
    public int debrisCount;
    public float spawnWait, startWait, waveWait;
    public static GameController instance = null;
    public GameObject[] panelLetters;
    public GameObject[] targetStandard;

    public Slider levelUpBar;
    public Slider healthBar;
    public Text currentRankText;
    public GameObject UIRoundBegin;
    public GameObject UIRoundOver;
    public GameObject player;
    public GameObject xpAddedText;
    public GameObject healthChangedText;
    public GameObject levelUnlockedText;
    public GameObject doubleBoltStatusIcon;
    public GameObject armorStatusIcon;
    public GameObject teleportStatusIcon;
    public GameObject background;

    public AudioClip healthPickup;
    public AudioClip teleportPickup;
    public AudioClip armorPickup;
    public AudioClip armorActivated;
    public AudioClip teleportActivated;
    public AudioClip dualShotPickup;

    private DataController dataController;
    private bool panelSet;
    private String targetWord;
    private int currentGameLevel;
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color completedColor = new Color32(212, 175, 55, 255);
    private String[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K", "L", "M", "N", "O", "P", "Q", "R","S", "T", "U", "V", "W", "X", "Y", "Z"
        };

    private String difficulty;
    private GameObject[] debrisArray;
    private int targetIndex;
    private int[] targetIndices;
    private AudioSource _audio;
    private AudioClip wordClip;
    private AudioClip introClip;
    private int currentRank;
    private LevelOfDifficulty currentDifficulty;

    private int experiencePoints;
    //private int experienceNeededToLevelUp;
    private int xpAdded;
    private int wordExperienceModifier = 1;
    private bool gameOver;
    private Color normalColor = Color.white;
    private Color hitColor = Color.red;
    private Color bufferColor = Color.yellow;
    private Color slowDownColor = Color.black;

    // player performance
    private float health = 1;
    private float healthMax = 1;

    [HideInInspector]
    public bool doubleBoltAbility;
    [HideInInspector]
    public bool armorAbility;
    [HideInInspector]
    public bool teleportAbility;

    public float flashDelay = 0.125f;
    public float slowFlashDelay = 0.25f;
    public int timesToFlash = 3;
    [HideInInspector]
    public bool isDead;
    [HideInInspector]
    public bool isPaused;
    public GameObject hangarButton;

    private float playerStreak = 0f;
    private const float streakModifier = 0.05f;
    private int targetWordIndex;
    public float startCountdown;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        dataController = FindObjectOfType<DataController>();

        _audio = GetComponent<AudioSource>();
        currentGameLevel = PlayerPrefs.GetInt("Level");
        targetWord = RandomWord();
        wordClip = Resources.Load<AudioClip>(dataController.allLevelData[currentGameLevel].words[targetWordIndex].audioPath);
        //introClip = Resources.Load<AudioClip>("Audio/intro");
        difficulty = dataController.playerData.difficultySelected;
        experiencePoints = dataController.GetPlayerXP();
        currentRank = dataController.GetPlayerRank();
        currentDifficulty = dataController.GetCurrentDifficulty();
        healthMax = GetHealthMax();
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            health = PlayerPrefs.GetFloat("PlayerHealth");
        }
        else
        {
            health = healthMax;
        }
        hangarButton.GetComponent<Button>().interactable = false;
        SetLevelBackground();

        if (PlayerPrefs.HasKey("PlayerStreak")){
            playerStreak = PlayerPrefs.GetFloat("PlayerStreak");
        }
        else
        {
            playerStreak = 0;
        }
        Time.timeScale = 1f;
        _audio.clip = wordClip;
    }

    private void SetLevelBackground()
    {
        Texture nebulaBackground = GetBackGroundTexture(currentGameLevel);
        GameObject backgroundChild = background.transform.GetChild(0).gameObject;
        backgroundChild.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
        background.GetComponent<Renderer>().material.mainTexture = nebulaBackground;
    }

    private Texture GetBackGroundTexture(int gameLevel)
    {
        return Resources.Load<Texture>(dataController.allLevelData[currentGameLevel].backgroundPath);
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
        gameOver = false;
        isPaused = true;
        wordExperienceModifier = currentGameLevel + 1;
        UIRoundBegin.SetActive(true);
        player.SetActive(false);
        StartCoroutine(Countdown(startCountdown));
        targetIndex = 0;
        StartCoroutine(SpawnWaves());
    }

    IEnumerator Countdown(float seconds)
    {
        float counter = seconds;
        while (counter >= 0)
        {
            yield return new WaitForSeconds(1);
            UIRoundBegin.GetComponent<Text>().text = counter + "";
            counter--;

        }
        
        UIRoundBegin.SetActive(false);
        player.SetActive(true);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
                Time.timeScale = 0f;
                //button_hangar_active
                hangarButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_hangar_active");
                hangarButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                hangarButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_hangar");
                hangarButton.GetComponent<Button>().interactable = false;
                pickupHelpMessage.SetActive(false);
                isPaused = false;
                Time.timeScale = 1f;
            }
        }
    }

    public String GetRankText(int rank)
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
        // on pause ui, player clicks button to continue which will set Time.timeScale = 1f
        Time.timeScale = 1f;
        UIRoundBegin.SetActive(false);
        player.SetActive(true);
        pickupHelpMessage.SetActive(false);
        PlayerPrefs.SetInt("InRound", 1);
        _audio.clip = wordClip;
        isPaused = false;
    }

    public void IncreaseHealth(float amt)
    {
        _audio.clip = healthPickup;
        _audio.Play();

        if (health == healthMax)
        {
            return;
        }
        else
        {
            health += amt;
            if (health > healthMax)
                health = healthMax;
        }
        StartCoroutine(HandleHealthText(3, amt, false));
        RefreshHealthBar();
    }

    public void DecreaseHealth(float damageAmt)
    {
        if (health > 0.5f)
        {
            health -= damageAmt;
            if (health <= 0)
                isDead = true;
            else
            {
                StartCoroutine(BeenHit());
            }
        }
        else
        {
            health = 0;
            isDead = true;
        }

        StartCoroutine(HandleHealthText(3, damageAmt, true));
        RefreshHealthBar();
    }

    public IEnumerator HandleHealthText(int delay, float amt, bool damage)
    {

        healthChangedText.SetActive(true);
        if (damage)
        {
            healthChangedText.GetComponent<Text>().text = "-" + amt + " HP";
        }
        else
        {
            healthChangedText.GetComponent<Text>().text = "+" + amt + " HP";
        }
        healthChangedText.GetComponent<Text>().CrossFadeAlpha(0, 3.0f, false);
        yield return new WaitForSeconds(delay);
        healthChangedText.GetComponent<Text>().CrossFadeAlpha(1, 0.0f, false);
        healthChangedText.SetActive(false);
    }
    public void ArmorActivated()
    {
        armorStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_deactive");
        _audio.clip = armorActivated;
        _audio.Play();
        StartCoroutine(ArmorAbilityOn());
        armorAbility = false;
    }

    public void LevelUp()
    {
        healthMax = GetHealthMax();
        health = healthMax;
        PlayerPrefs.DeleteKey("PlayerHealth");
    }

    public void HealthPickup()
    {
        int num = UnityEngine.Random.Range(1, 6);
        switch (num)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                if (health < 3.0f)
                {
                    health += 0.5f;
                }
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

    public void RoundLose()
    {
        gameOver = true;
        UIRoundOver.GetComponent<Text>().text = "Better luck next time, " + currentRankText.text + ". Tap to continue.";
        UIRoundOver.SetActive(true);
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
    }

    public void ContinuePlaying()
    {
        SceneManager.LoadScene("Game");
    }

    public void PlayWord()
    {
        _audio.clip = wordClip;
        _audio.Play();
    }

    public void GoHome()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.SetInt("InRound", 0);
        SceneManager.LoadScene("MainMenu");
    }

    public Boolean ProcessHit(String hitLetter)
    {
        Boolean goodHit = false;
        if (targetIndex < targetWord.Length)
        {
            String targetLetter = "";
            if (targetIndex < targetWord.Length - 1)
                targetLetter = targetWord.Substring(targetIndex, 1);
            else
                targetLetter = targetWord.Substring(targetIndex);

            if (targetLetter.Equals(hitLetter))
            {
                goodHit = true;

                //mark complete
                GameObject[] targetPanel = GetTargetPanel();
                int targetPanelIndex = CalculateTargetPanelIndex();
                int elementIndex = 0;
                elementIndex = targetIndex - (targetPanel.Length * targetPanelIndex);
                targetPanel[elementIndex].SetActive(true);
                targetPanel[elementIndex].GetComponent<Image>().color = completedColor;

                if (targetIndex == targetWord.Length - 1)
                {
                    RoundWin();
                }
                else
                {
                    targetIndex++;
                }
            }
            else
            {
                StartCoroutine(DecreaseSpeed());
                StartCoroutine(SlowDownEffect());
            }
        }
        return goodHit;
    }
    public void SpawnRandomPickup(GameObject[] pickups, Transform pickupTransform, Quaternion rotateQuaternion)
    {
        int num = UnityEngine.Random.Range(1, 6);

        switch (num)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                int pickupNum = 0;
                if (currentRank > DataController.CHIEF_RANK)
                    pickupNum = UnityEngine.Random.Range(0, 4);
                else if (currentRank > DataController.PILOT_RANK)
                    pickupNum = UnityEngine.Random.Range(0, 3);
                else if (currentRank > DataController.CADET_RANK)
                    pickupNum = UnityEngine.Random.Range(0, 2);

                bool firstPickup = IsFirstTimeForPickup(pickupNum);
                if (firstPickup) // if first pickup, need to show a msg with pickup
                {
                    Instantiate(pickups[pickupNum], pickupTransform.position, rotateQuaternion);

                    // determine msg text
                    if (pickupNum == 0)
                    {
                        pickupHelpMessage.GetComponent<Text>().text = "A 'health pickup' restores 1 point to HP.\n\nTap to continue.";
                        PlayerPrefs.SetString("HEALTHPICKUP", "YES");
                    }
                    else if (pickupNum == 1)
                    {
                        pickupHelpMessage.GetComponent<Text>().text = "A 'dual shot pickup' shoots two lasers next fire.\n\nTap to continue.";
                        PlayerPrefs.SetString("DUALSHOTPICKUP", "YES");
                    }
                    else if (pickupNum == 2)
                    {
                        pickupHelpMessage.GetComponent<Text>().text = "An 'armor pickup' prevents damage next hit.\n\nTap to continue.";
                        PlayerPrefs.SetString("ARMORPICKUP", "YES");
                    }
                    else if (pickupNum == 3)
                    {
                        pickupHelpMessage.GetComponent<Text>().text = "A 'teleport pickup' moves your ship to where you tap on screen.\n\nTap to continue.";
                        PlayerPrefs.SetString("TELEPORTPICKUP", "YES");
                    }

                    // show msg
                    pickupHelpMessage.SetActive(true);
                    isPaused = true;
                    Time.timeScale = 0f; // freeze game
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
            case 0: // Health
                return !PlayerPrefs.HasKey("HEALTHPICKUP");
            case 1: // Dual Shot
                return !PlayerPrefs.HasKey("DUALSHOTPICKUP");
            case 2: // Armor
                return !PlayerPrefs.HasKey("ARMORPICKUP");
            case 3: // Teleport
                return !PlayerPrefs.HasKey("TELEPORTPICKUP");
        }
        return true;
    }

    private void RefreshUI()
    {
        currentRankText.text = GetRankText(currentRank);
        levelUpBar.maxValue = (int)CalculateRankXP(currentRank);
        levelUpBar.value = experiencePoints;
        RefreshHealthBar();
    }

    private void RefreshHealthBar()
    {
        healthBar.maxValue = healthMax;
        healthBar.value = health;
    }

    private float GetHealthMax()
    {
        switch (currentRank)
        {
            case DataController.RECRUIT_RANK:
                return 1.0f;
            case DataController.CADET_RANK:
                return 2.0f;
            case DataController.PILOT_RANK:
                return 3.0f;
            case DataController.ACE_RANK:
                return 4.0f;
            case DataController.CHIEF_RANK:
                return 5.0f;
            case DataController.CAPTAIN_RANK:
                return 6.0f;
            case DataController.COMMANDER_RANK:
                return 7.0f;
            case DataController.MASTER_RANK:
                return 8.0f;
            default:
                return 1.0f;
        }
    }

    private double CalculateRankXP(int rank)
    {
        double exponent = 1.5;
        double baseXP = 50;
        return Math.Floor(baseXP * Math.Pow(rank + 1, exponent));
    }

    private void CheckProgression()
    {
        int currentRankXP = (int)CalculateRankXP(currentRank);
        if (experiencePoints > currentRankXP)
        {
            currentRank++;
            experiencePoints = experiencePoints - currentRankXP;
            LevelUp();

        }
        dataController.SavePlayerProgress(currentRank, experiencePoints, currentGameLevel, targetWord);
        List<string> completedLevelList = dataController.getCompletedLevelList(currentGameLevel);
        if (currentGameLevel != 9 && !currentDifficulty.levelsUnlocked[currentGameLevel + 1] && completedLevelList.Count >= ((dataController.allLevelData[currentGameLevel].words.Length / 2) + 1)) // at least 51% of the words spelled correctly, then mark complete
        {
            dataController.UnlockNextLevel(currentGameLevel);
            levelUnlockedText.SetActive(true);
        }
    }

    private string RandomWord()
    {
        WordData[] words = dataController.allLevelData[currentGameLevel].words;
        targetWordIndex = UnityEngine.Random.Range(0, words.Length);
        return words[targetWordIndex].word;
    }

    private void CalculateWordScore()
    {
        double xpEarned = Math.Round (targetWord.Length * (wordExperienceModifier + playerStreak), MidpointRounding.AwayFromZero);
        xpAdded = (int)xpEarned;
        experiencePoints = experiencePoints + xpAdded;
    }

    private void RoundWin()
    {
        CalculateWordScore();
        CheckProgression();
        RefreshUI();
        gameOver = true;
        UIRoundOver.GetComponent<Text>().text = "Good job, " + currentRankText.text + ". Click to continue.";
        UIRoundOver.SetActive(true);
        xpAddedText.SetActive(true);
        xpAddedText.GetComponent<Text>().text = "+" + xpAdded + " xp ";
        xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 6.0f, true);
        player.SetActive(false);
        PlayerPrefs.SetFloat("PlayerHealth", health);
        PlayerPrefs.SetFloat("PlayerStreak", playerStreak + streakModifier);
    }

    private void PopulateDebrisArray()
    {
        debrisArray = new GameObject[debrisCount];
        GameObject[] blocksArray = null;

        if (targetIndex < 9)
            blocksArray = new GameObject[9];
        else
        {
            UpdateActivePanel(targetStandard, CalculateTargetPanelIndex());
            blocksArray = new GameObject[9];
        }

        blocksArray[0] = blocks[targetIndices[targetIndex]]; // word mode

        for (int j = 1; j < blocksArray.Length; j++)
        {
            int num = UnityEngine.Random.Range(0, 26);
            blocksArray[j] = blocks[num];
        }

        for (int i = 0; i < debrisCount; i++)
        {
            int random = UnityEngine.Random.Range(0, 3);
            if (random == 0) // choose hazard
            {
                int numHazards = 0;
                switch (currentGameLevel)
                {
                    case 0:
                    case 1:
                        numHazards = 1;
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        numHazards = 2;
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        numHazards = 3;
                        break;
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        numHazards = 4;
                        break;
                }
                debrisArray[i] = hazards[UnityEngine.Random.Range(0, numHazards)];
            }
            else // choose block
            {
                int num = UnityEngine.Random.Range(0, 100);
                if (num <= 29) // 30 % chance
                {
                    debrisArray[i] = blocksArray[0];
                }
                else if (num > 29 && num <= 44) // 15 % chance
                {
                    debrisArray[i] = blocksArray[1];
                }
                else if (num > 44 && num <= 59) // 10 % chance
                {
                    debrisArray[i] = blocksArray[2];
                }
                else if (num > 59 && num <= 69) // 10 % chance
                {
                    debrisArray[i] = blocksArray[3];
                }
                else if (num > 69 && num <= 79) // 10 % chance
                {
                    debrisArray[i] = blocksArray[4];
                }
                else if (num > 79 && num <= 84) // 5 % chance
                {
                    debrisArray[i] = blocksArray[5];
                }
                else if (num > 84 && num <= 89) // 5 % chance
                {
                    debrisArray[i] = blocksArray[6];
                }
                else if (num > 89 && num <= 94) // 5 % chance
                {
                    debrisArray[i] = blocksArray[7];
                }
                else if (num > 94) // 5 % chance
                {
                    debrisArray[i] = blocksArray[8];
                }

            }
        }

    }

    private int CalculateTargetPanelIndex()
    {
        if (targetIndex < 9)
            return 0;
        else if (targetIndex > 8 && targetIndex < 18)
            return 1;
        else
            return 2;
    }

    private GameObject[] GetTargetPanel()
    {
        return targetStandard;
    }

    private void SetActivePanel(GameObject[] panel, bool status)
    {
        if (panel[0].activeSelf == status)
            return;
        for (int j = 0; j < panel.Length; j++)
        {
            panel[j].SetActive(status);
        }
    }

    private void UpdateActivePanel(GameObject[] panel, int targetPanelIndex)
    {
        if (targetPanelIndex == 0 || (targetIndex > 9 && targetIndex < 18) || (targetIndex > 18))
            return;
    }
    private int[] CalculateTargetIndices()
    {
        int[] targetIndices = new int[targetWord.Length];
        String letter = targetWord.Substring(0, 1);
        int letterIndex = 0;
        while (letterIndex < targetWord.Length)
        {
            for (int j = 0; j < letters.Length; j++)
            {
                if (letter.Equals(letters[j]))
                {
                    targetIndices[letterIndex] = j;
                    letterIndex++;
                    if (letterIndex < targetWord.Length - 1)
                        letter = targetWord.Substring(letterIndex, 1);
                    else
                        letter = targetWord.Substring(letterIndex);
                }
            }

        }
        return targetIndices;
    }

    IEnumerator DecreaseSpeed()
    {
        PlayerController.instance.speed = PlayerController.instance.speed / 2;
        yield return new WaitForSeconds(3.0f);
        PlayerController.instance.speed = PlayerController.instance.originalSpeed;
    }

    IEnumerator BeenHit()
    {
        if (player != null && player.activeSelf)
        {
            for (int i = 1; i <= timesToFlash; i++)
            {
                if (player != null && player.activeSelf)
                {
                    player.GetComponent<Renderer>().material.color = hitColor;
                    yield return new WaitForSeconds(flashDelay);
                }
                if (player != null && player.activeSelf)
                {
                    player.GetComponent<Renderer>().material.color = normalColor;
                    yield return new WaitForSeconds(flashDelay);
                }
            }
        }
    }

    public IEnumerator ArmorAbilityOn()
    {
        for (int i = 1; i <= timesToFlash; i++)
        {
            player.GetComponent<Renderer>().material.color = bufferColor;
            yield return new WaitForSeconds(flashDelay);
            player.GetComponent<Renderer>().material.color = normalColor;
            yield return new WaitForSeconds(flashDelay);
        }
        
    }

    public IEnumerator SlowDownEffect()
    {
        for (int i = 1; i <= timesToFlash; i++)
        {
            player.GetComponent<Renderer>().material.color = slowDownColor;
            yield return new WaitForSeconds(slowFlashDelay);
            player.GetComponent<Renderer>().material.color = normalColor;
            yield return new WaitForSeconds(slowFlashDelay);
        }
    }


    public void TeleportActivated()
    {
        //teleportStatusIcon.SetActive(false);
        teleportStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_deactive");
        //wormholeIcon.color = defaultColor;
        _audio.clip = teleportActivated;
        _audio.Play();
    }

    public void TeleportPickup()
    {
        //wormholeIcon.color = completedColor;
        teleportStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        //teleportStatusIcon.SetActive(true);
        teleportAbility = true;
        _audio.clip = teleportPickup;
        _audio.Play();
    }

    public void ArmorPickup()
    {
        //armorStatusIcon.SetActive(true);
        armorStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        armorAbility = true;
        _audio.clip = armorPickup;
        _audio.Play();
    }

    public void ResetDualShot()
    {
        //doubleBoltStatusIcon.SetActive(false);
        doubleBoltStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_deactive");
        //doubleBoltIcon.color = defaultColor;
        doubleBoltAbility = false;
    }

    public void DualShotPickup()
    {
        doubleBoltStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        doubleBoltAbility = true;
        _audio.clip = dualShotPickup;
        _audio.Play();
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait + startCountdown);
        StartCoroutine(DisplayWord(10.0f));

        _audio.Play();
        while (true)
        {
            PopulateDebrisArray();

            for (int i = 0; i < debrisCount; i++)
            {
                GameObject debris = debrisArray[UnityEngine.Random.Range(0, debrisArray.Length)];
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnValues.x, spawnValues.x),
                    spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(debris, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                break;
            }

        }
    }

    IEnumerator DisplayWord(float delay)
    {
        if (!difficulty.Equals(DataController.DIFFICULTY_HARD)) // if not hard, show the word
        {
            targetIndices = CalculateTargetIndices();

            for (int i = 0; i < targetStandard.Length; i++)
            {
                if (i < targetIndices.Length)
                {
                    targetStandard[i].GetComponent<Image>().sprite = panelLetters[targetIndices[i]].GetComponent<Image>().sprite;
                    targetStandard[i].SetActive(true);
                }
                else
                {
                    targetStandard[i].SetActive(false);
                }
            }
            if (difficulty.Equals(DataController.DIFFICULTY_NORMAL)) //if normal, after showing the word delay for set time and then turn off word
            {
                yield return new WaitForSeconds(delay);
                targetIndices = CalculateTargetIndices();

                for (int i = 0; i < targetStandard.Length; i++)
                {
                    if (i < targetIndices.Length)
                    {
                        targetStandard[i].GetComponent<Image>().sprite = panelLetters[targetIndices[i]].GetComponent<Image>().sprite;
                        targetStandard[i].SetActive(false);
                    }
                    else
                    {
                        targetStandard[i].SetActive(false);
                    }
                }
            }
        }
        else // else, never show the word
        {
            targetIndices = CalculateTargetIndices();

            for (int i = 0; i < targetStandard.Length; i++)
            {
                if (i < targetIndices.Length)
                {
                    targetStandard[i].GetComponent<Image>().sprite = panelLetters[targetIndices[i]].GetComponent<Image>().sprite;
                    targetStandard[i].SetActive(false);
                }
                else
                {
                    targetStandard[i].SetActive(false);
                }
            }
        }
    }


}