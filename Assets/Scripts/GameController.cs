﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public GameObject[] blocks;
    public Vector3 spawnValues;
    public int debrisCount;
    public float spawnWait, startWait, waveWait;
    public static GameController instance = null;
    public GameObject[] healthIndicator;
    public GameObject[] panelLetters;
    public GameObject[] targetStandard;

    public Slider levelUpBar;
    public Text currentRankText;
    public GameObject UIRoundBegin;
    public GameObject UIRoundOver;
    public GameObject player;
    public GameObject xpAddedText;

    public AudioClip healthPickup;

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

    private int experiencePoints;
    private int experienceNeededToLevelUp;
    private int wordExperienceModifier = 2;
    private bool gameOver;
    private Color normalColor = Color.white;
    private Color hitColor = Color.red;
    private Color bufferColor = Color.yellow;

    // player performance
    private float health = 2;
    private float healthMax = 2;

    [HideInInspector]
    public bool doubleBoltAbility;
    [HideInInspector]
    public bool bufferAbility;
    [HideInInspector]
    public bool wormholeAbility;

    public float flashDelay = 0.125f;
    public int timesToFlash = 3; 
    [HideInInspector]
    public bool isDead;
    [HideInInspector]
    public bool isPaused;
    public GameObject homeButton;

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
        wordClip = Resources.Load<AudioClip>("Audio/" + currentGameLevel + "/" + targetWord.ToLower());
        introClip = Resources.Load<AudioClip>("Audio/intro");
        difficulty = PlayerPrefs.GetString("Difficulty");
        experiencePoints = PlayerPrefs.GetInt("XP");
        currentRank = PlayerPrefs.GetInt("Rank");
        healthMax = CalculateHealthMax();
        health = healthMax;
        homeButton.GetComponent<Button>().interactable = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
        gameOver = false;
        isPaused = false;
        if (PlayerPrefs.GetInt("InRound") == 0)
        {
            isPaused = true;
            UIRoundBegin.SetActive(true);

            player.SetActive(false);
            Time.timeScale = 0f;
            _audio.clip = introClip;
            _audio.Play();
        }
        else
        {
            _audio.clip = wordClip;
        }
        targetIndex = 0;
        StartCoroutine(SpawnWaves());
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
                homeButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                homeButton.GetComponent<Button>().interactable = false;
                isPaused = false;
                Time.timeScale = 1f;
            }
        }
    }

    public String GetRankText(int rank)
    {
        switch (rank)
        {
            case DataController.PRESCHOOL_RANK:
                return "Preschooler";
            case DataController.KINDERGARTEN_RANK:
                return "Kindergartner";
            case DataController.FIRSTGRADE_RANK:
                return "First Grader";
            case DataController.SECONDGRADE_RANK:
                return "Second Grader";
            case DataController.THIRDGRADE_RANK:
                return "Third Grader";
            case DataController.FOURTHGRADE_RANK:
                return "Fourth Grader";
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
        PlayerPrefs.SetInt("InRound", 1);
        _audio.clip = wordClip;
        isPaused = false;
    }

    public void IncreaseHealth(float amt)
    {
        _audio.clip = healthPickup;
        _audio.Play();
        if (health == healthMax)
            return;
        else
        {
            health += amt;
            if (health > healthMax)
                health = healthMax;
            RefreshUI();
        }
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
                int healthLevel = (int)(health * 2) - 1;
                int index = healthIndicator.Length - 1;

                while (index > healthLevel)
                {
                    healthIndicator[index].SetActive(false);
                    index--;
                }
            
        }
        else
        {
            health = 0;
            healthIndicator[0].SetActive(false);
            isDead = true;
        }
    }

    public void ShieldsUp()
    {
        StartCoroutine(BufferedAbilityOn());
        bufferAbility = false;
    }

    public void LevelUp(bool increaseHealth, float amt)
    {
        if (increaseHealth)
        {
            healthMax = healthMax + amt;
            if (healthMax > 3)
                healthMax = 3;
            health = healthMax;
            PlayerPrefs.SetFloat("PlayerHealthMax", healthMax);
        }
        else
        {
            health = healthMax;
            PlayerPrefs.SetFloat("PlayerHealthMax", healthMax);
        }
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

    public void GameLose()
    {
        CalculateWordScore(false);
        gameOver = true;
        UIRoundOver.GetComponent<Text>().text = "Better luck next time, cadet. Click to continue playing.";
        UIRoundOver.SetActive(true);
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
                    GameWin();
                }
                else
                {
                    targetIndex++;
                }
            }
            else
            {
                StartCoroutine(DecreaseSpeed());
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
                if (currentRank > DataController.NINTHGRADE_RANK)
                    Instantiate(pickups[UnityEngine.Random.Range(0, 4)], pickupTransform.position, rotateQuaternion);
                else if (currentRank > DataController.FIFTHGRADE_RANK)
                    Instantiate(pickups[UnityEngine.Random.Range(0, 3)], pickupTransform.position, rotateQuaternion);
                else if (currentRank > DataController.KINDERGARTEN_RANK)
                    Instantiate(pickups[UnityEngine.Random.Range(0, 2)], pickupTransform.position, rotateQuaternion);
                else
                    Instantiate(pickups[0], pickupTransform.position, rotateQuaternion);
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

    private void RefreshUI()
    {
        currentRankText.text = GetRankText(currentRank);
        levelUpBar.maxValue = (int)CalculateRankXP(currentRank);
        levelUpBar.value = experiencePoints;

        int healthLevel = (int)(health * 2);
        for (int i = 0; i < healthLevel; i++)
        {
            healthIndicator[i].SetActive(true);
        }
    }

    private float CalculateHealthMax()
    {
        switch (currentRank)
        {
            case 0:
            case 1:
                return 2.0f;
            case 2:
                return 2.5f;
            case 3:
                return 3.0f;
            case 4:
                return 3.5f;
            case 5:
                return 4.0f;
            case 6:
                return 4.5f;
            case 7:
                return 5.0f;
            case 8:
                return 5.5f;
            case 9:
                return 6.0f;
            case 10:
                return 6.5f;
            case 11:
                return 7.0f;
            case 12:
                return 7.5f;
            case 13:
                return 8.0f;
            default:
                return 2.0f;
                    
        }
    }

    private double CalculateRankXP(int rank)
    {
        double exponent = 1.25;
        double baseXP = 10;
        return Math.Floor(baseXP * Math.Pow(rank+1, exponent));
    }

    private void CheckProgression()
    {
        int currentRankXP = (int)CalculateRankXP(currentRank);
        if (experiencePoints > currentRankXP)
        {
            currentRank++;
            experiencePoints = experiencePoints - currentRankXP;
            if (currentRank > 1)
            {
                LevelUp(true, 0.5f);
            }
            else
            {
                LevelUp(false, 0f);
            }
        }
        dataController.SavePlayerProgress(currentRank, experiencePoints);
    }

    private string RandomWord()
    {
        WordData[] words = dataController.allLevelData[currentGameLevel].words;
        return words[UnityEngine.Random.Range(0, words.Length)].word;
    }

    private void CalculateWordScore(bool won)
    {
        if (won)
        {
            experiencePoints = experiencePoints + (targetWord.Length * wordExperienceModifier);
            CheckProgression();
            RefreshUI();
        }
    }

    private void GameWin()
    {
        CalculateWordScore(true);
        gameOver = true;
        UIRoundOver.GetComponent<Text>().text = "Good job, cadet. You've just earned <color=#42f442>" + (targetWord.Length * wordExperienceModifier) + " experience points.</color> Click to continue.";
        UIRoundOver.SetActive(true);
        xpAddedText.SetActive(true);
        xpAddedText.GetComponent<Text>().text = "+" + (targetWord.Length * wordExperienceModifier) + " xp ";
        xpAddedText.GetComponent<Text>().CrossFadeAlpha(0, 6.0f, true);
        player.SetActive(false);
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
        for (int i = 1; i <= timesToFlash; i++)
        {
            if (player.activeSelf)
            {
                player.GetComponent<Renderer>().material.color = hitColor;
                yield return new WaitForSeconds(flashDelay);
                player.GetComponent<Renderer>().material.color = normalColor;
                yield return new WaitForSeconds(flashDelay);
            }
        }
    }

    IEnumerator BufferedAbilityOn()
    {
        for (int i = 1; i <= timesToFlash; i++)
        {
            player.GetComponent<Renderer>().material.color = bufferColor;
            yield return new WaitForSeconds(flashDelay);
            player.GetComponent<Renderer>().material.color = normalColor;
            yield return new WaitForSeconds(flashDelay);
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
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