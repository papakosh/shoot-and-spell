using System;
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
    //public Text progressText;
    public String targetWord;
    public GameObject[] healthIndicator;
    public GameObject[] targetPanel1;
    public GameObject[] targetPanel2;
    public GameObject[] targetPanel3;
    public String gameMode = MODE_ALPHABET;
    public GameObject[] panelLetters;
    private bool panelSet;
    private String gradeLevel;
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color completedColor = new Color32(212, 175, 55, 255);
    private String[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K", "L", "M", "N", "O", "P", "Q", "R","S", "T", "U", "V", "W", "X", "Y", "Z"
        };

    private String difficulty;
    private GameObject[] debrisArray;
    public int targetIndex;
    const String MODE_ALPHABET = "ALPHABET";
    const String MODE_WORD = "WORD";
    public GameObject[] targetStandard;
    private int[] targetIndices;
    private AudioSource _audio;
    private AudioClip wordClip;
    private string[] preschool = {"AWAY", "BIG", "BLUE", "CAN", "FIND", "FUNNY", "HELP", "MAKE", "LOOK", "PLAY"};
    private string[] kindergarten = { "ARTIST", "COMMUNITY", "DENTIST", "DEW", "GLANCE", "GUST", "KNIGHT", "LAUNDRY", "NOTE", "PRESIDENT"};
    private string[] first = { "APPLAUSE","BLUSH", "BORROW", "CABIN", "CAVE", "ENORMOUS", "MOUNTAIN", "NARROW", "SIBLING", "THUNDER"};
    private string[] second = { "ASTRONOMY", "TELESCOPE", "CLIFF", "DANGEROUS", "FUEL", "INSECT", "NERVOUS", "PLANET", "SHELTER", "VILLAGE"};
    private string[] third = {"ADOPT", "ARCTIC", "CUSTOM", "EXAMINE", "LOYAL", "NECTAR", "PASSAGE", "PREDATOR", "SCHEDULE", "TREASURE"};
    private string[] fourth = {"ARENA", "ASCEND", "CONFUSE", "CREATE", "FRONTIER", "HOST", "MATURE","PORTION", "SHABBY", "VALIANT"};
    int skillLevel;
    public const int PRESCHOOL_SKILLEVEL = 1;
    public const int KINDERGARTEN_SKILLEVEL = 2;
    public const int FIRSTGRADE_SKILLEVEL = 3;
    public const int SECONDGRADE_SKILLEVEL = 4;
    public const int THIRDGRADE_SKILLEVEL = 5;
    public const int FOURTHGRADE_SKILLEVEL = 6;
    private int[] skillLevelArray = { PRESCHOOL_SKILLEVEL, KINDERGARTEN_SKILLEVEL, FIRSTGRADE_SKILLEVEL, SECONDGRADE_SKILLEVEL, THIRDGRADE_SKILLEVEL, FOURTHGRADE_SKILLEVEL };

    int level;
    int experience;
    int experienceNeededToLevelUp;
    int wordExperienceModifier = 2;

    public Slider levelUpBar;
    public Text currentLevel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        for (int i = 0; i < healthIndicator.Length; i++)
        {
            healthIndicator[i].SetActive(true);
        }

        _audio = GetComponent<AudioSource>();
        if (PlayerPrefs.GetString("GameMode") != null && PlayerPrefs.GetString("GameMode").Length > 0)
        {
            gameMode = PlayerPrefs.GetString("GameMode");
            gradeLevel = PlayerPrefs.GetString("GradeLevel");
            targetWord = RandomWord();
            wordClip = Resources.Load<AudioClip>("Audio/" + gradeLevel + "/" + targetWord.ToLower());
            difficulty = PlayerPrefs.GetString("GameDifficulty");
            experience = PlayerPrefs.GetInt("XP");
            skillLevel = PlayerPrefs.GetInt("SkillLevel");
        }

    }

    public String GetSkillLevelText(int level)
    {
        switch (level)
        {
            case 1:
                return "Preschooler";
            case 2:
                return "Kindergartner";
            case 3:
                return "First Grader";
            case 4:
                return "Second Grader";
            case 5:
                return "Third Grader";
            case 6:
                return "Fourth Grader";
            default:
                return "Undefined";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLevel.text = GetSkillLevelText(skillLevel);
        PlayerPrefs.SetInt("FirstTime", 1);

        // pause game design
        // show paused ui
        // pause game by setting Time.timeScale = 0f
        // on pause ui, player clicks button to continue which will set Time.timeScale = 1f
        StartCoroutine(DisplayWord(10.0f));
        _audio.clip = wordClip;
        _audio.Play();
        targetIndex = 0;
        StartCoroutine(SpawnWaves());

        levelUpBar.maxValue = (int)nextLevelCustom(skillLevel);
        levelUpBar.value = experience;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private double nextLevelCustom(int level)
    {
        double exponent = 1.25;
        double baseXP = 50;
        return Math.Floor(baseXP * Math.Pow(level, exponent));
    }

    private void CheckProgression()
    {
         
        if (experience > (int)nextLevelCustom(skillLevel))
        {
            skillLevel++;
            experience = 0;
        }
    }

    private string RandomWord ()
    {
        switch (gradeLevel)
        {
            case "Preschool":
                return preschool[UnityEngine.Random.Range(0, preschool.Length)];
            case "Kindergarten":
                return kindergarten[UnityEngine.Random.Range(0, kindergarten.Length)];
            case "FirstGrade":
                return first[UnityEngine.Random.Range(0, first.Length)];
            case "SecondGrade":
                return second[UnityEngine.Random.Range(0, second.Length)];
            case "ThirdGrade":
                return third[UnityEngine.Random.Range(0, third.Length)];
            case "FourthGrade":
                return fourth[UnityEngine.Random.Range(0, fourth.Length)];
            default:
                return null;
        }
    }

    public void PlayWord()
    {
        _audio.Play();
    }

    public void GoHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator CalculateWordScore (bool won)
    {
        if (won)
        {
            experience = experience + (targetWord.Length * wordExperienceModifier);
            CheckProgression();
            PlayerPrefs.SetInt("XP", experience);
            PlayerPrefs.SetInt("SkillLevel", skillLevel);
        }

        yield return new WaitForSeconds(5.0f);

    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            PopulateDebrisArray();

            for (int i = 0; i < debrisCount; i++)
            {
                GameObject debris = debrisArray[UnityEngine.Random.Range(0, debrisArray.Length)];
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnValues.x, spawnValues.x),
                    spawnValues.y,spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(debris, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);
        }
    }

    IEnumerator DisplayWord(float delay)
    {
        

        if (!difficulty.Equals("HARD")) // if not hard, show the word
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
            if (difficulty.Equals("NORMAL")) //if normal, after showing the word delay for set time and then turn off word
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

    private void GameWin()
    {
        StartCoroutine(CalculateWordScore(true));
        SceneManager.LoadScene("Game");
    }

    public void GameLose()
    {
        StartCoroutine(CalculateWordScore(false));
        SceneManager.LoadScene("Game");
    }
   
    public Boolean inAlphabetMode()
    {
        return gameMode.Equals(MODE_ALPHABET);
    }

    private void PopulateDebrisArray()
    {
        debrisArray = new GameObject[debrisCount];
        GameObject[] blocksArray = null;
                
       if (targetIndex < 9)
            blocksArray = new GameObject[9];
        else{
            UpdateActivePanel(targetStandard, CalculateTargetPanelIndex());
            blocksArray = new GameObject[9];
        }

        if (gameMode.Equals(MODE_ALPHABET))
        {
            blocksArray[0] = blocks[targetIndex]; //alphabet mode
        }
        else
        {
            blocksArray[0] = blocks[targetIndices[targetIndex]]; // word mode
        }

        for (int j = 1; j < blocksArray.Length; j++)
        {
            int num = UnityEngine.Random.Range(0,26);
            blocksArray[j] = blocks[num];
        }

        for (int i = 0; i < debrisCount; i++)
        {
            int random = UnityEngine.Random.Range(0, 3);
            if (random == 0) // choose hazard
            {
                debrisArray[i] = hazards[UnityEngine.Random.Range(0, hazards.Length)];
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
                elementIndex= targetIndex - (targetPanel.Length * targetPanelIndex);
                targetPanel[elementIndex].SetActive(true);
                targetPanel[elementIndex].GetComponent<Image>().color= completedColor;
                
                if (targetIndex == targetWord.Length - 1) {
                    GameWin();
                }
                else
                {
                    targetIndex++;
                }
            }
            else
            {
                StartCoroutine(PlayerController.instance.DecreaseSpeed());
            }
        }
        return goodHit;
    }

    private int CalculateTargetPanelIndex()
    {
        if(targetIndex < 9)
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
        if (targetPanelIndex == 0 || (targetIndex >9 && targetIndex <18) || (targetIndex > 18))
            return;
        else if (targetPanelIndex == 1)
        {
            for (int i = 0; i < targetPanel2.Length; i++)
            {
                panel[i].GetComponent<Image>().sprite = targetPanel2[i].GetComponent<Image>().sprite;
                if (PlayerPrefs.GetString("GameDifficulty").Equals("EASY"))
                {
                    panel[i].SetActive(true);
                }
                else
                {
                    panel[i].SetActive(false);
                }
                panel[i].GetComponent<Image>().color = defaultColor;
            }
        }
        else if (targetPanelIndex == 2)
        {
            for (int i = 0; i < targetPanel3.Length; i++)
            {
                panel[i].GetComponent<Image>().sprite = targetPanel3[i].GetComponent<Image>().sprite;
                if (PlayerPrefs.GetString("GameDifficulty").Equals("EASY"))
                {
                    panel[i].SetActive(true);
                }
                else
                {
                    panel[i].SetActive(false);
                }
                panel[i].GetComponent<Image>().color = defaultColor;//null out last letter
            }
            panel[8].SetActive(false);
        }

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

}