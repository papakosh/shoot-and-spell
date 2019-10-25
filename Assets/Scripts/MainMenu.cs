using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject easyButton;
    public GameObject normalButton;
    public GameObject hardButton;
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;
    public GameObject level5;
    public GameObject level6;
    public GameObject level7;
    public GameObject level8;
    public GameObject level9;
    public GameObject level10;
    public Slider level1Progress;
    public Slider level2Progress;
    public Slider level3Progress;
    public Slider level4Progress;
    public Slider level5Progress;
    public Slider level6Progress;
    public Slider level7Progress;
    public Slider level8Progress;
    public Slider level9Progress;
    public Slider level10Progress;
    public Text level1ProgressText;
    public Text level2ProgressText;
    public Text level3ProgressText;
    public Text level4ProgressText;
    public Text level5ProgressText;
    public Text level6ProgressText;
    public Text level7ProgressText;
    public Text level8ProgressText;
    public Text level9ProgressText;
    public Text level10ProgressText;

    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color chosenColor = new Color32(212, 175, 55, 255);

    private DataController dataController;
    private Difficulty currentDifficulty;

    private AudioSource _audio;

    public void CallSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void CallInfo()
    {
        SceneManager.LoadScene("Info");
    }
    void Awake()
    {
        dataController = FindObjectOfType<DataController>();
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
       
    }

    private void Start()
    {
        currentDifficulty = dataController.currentDifficulty;
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
        PlayerPrefs.DeleteKey("DualShot");
        PlayerPrefs.DeleteKey("Armor");
        PlayerPrefs.DeleteKey("Teleport");

        level1.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[0];
        level2.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[1];
        level3.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[2];
        level4.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[3];
        level5.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[4];
        level6.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[5];
        level7.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[6];
        level8.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[7];
        level9.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[8];
        level10.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[9];

        List<string> completedLevelList = currentDifficulty.level1CompletedWords;
        if (level1Progress != null && level1Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[0].words.Length;
            int numberOfCompletedWords = completedLevelList.Count;
            level1Progress.maxValue = totalNumberOfWords;
            level1Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            level1ProgressText.text = progressValue;
        }

        List<string> completedLevelList2 = currentDifficulty.level2CompletedWords;
        if (level2Progress != null && level2Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[1].words.Length;
            int numberOfCompletedWords = completedLevelList2.Count;
            level2Progress.maxValue = totalNumberOfWords;
            level2Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[1])
                progressValue = "Locked";
            level2ProgressText.text = progressValue;
        }

        List<string> completedLevelList3 = currentDifficulty.level3CompletedWords;
        if (level3Progress != null && level3Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[2].words.Length;
            int numberOfCompletedWords = completedLevelList3.Count;
            level3Progress.maxValue = totalNumberOfWords;
            level3Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[2])
                progressValue = "Locked";
            level3ProgressText.text = progressValue;
        }

        List<string> completedLevelList4 = currentDifficulty.level4CompletedWords;
        if (level4Progress != null && level4Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[3].words.Length;
            int numberOfCompletedWords = completedLevelList4.Count;
            level4Progress.maxValue = totalNumberOfWords;
            level4Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[3])
                progressValue = "Locked";
            level4ProgressText.text = progressValue;
        }

        List<string> completedLevelList5 = currentDifficulty.level5CompletedWords;
        if (level5Progress != null && level5Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[4].words.Length;
            int numberOfCompletedWords = completedLevelList5.Count;
            level5Progress.maxValue = totalNumberOfWords;
            level5Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[4])
                progressValue = "Locked";
            level5ProgressText.text = progressValue;
        }

        List<string> completedLevelList6 = currentDifficulty.level6CompletedWords;
        if (level6Progress != null && level6Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[5].words.Length;
            int numberOfCompletedWords = completedLevelList6.Count;
            level6Progress.maxValue = totalNumberOfWords;
            level6Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[5])
                progressValue = "Locked";
            level6ProgressText.text = progressValue;
        }

        List<string> completedLevelList7 = currentDifficulty.level7CompletedWords;
        if (level7Progress != null && level7Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[6].words.Length;
            int numberOfCompletedWords = completedLevelList7.Count;
            level7Progress.maxValue = totalNumberOfWords;
            level7Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[6])
                progressValue = "Locked";
            level7ProgressText.text = progressValue;
        }

        List<string> completedLevelList8 = currentDifficulty.level8CompletedWords;
        if (level8Progress != null && level8Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[7].words.Length;
            int numberOfCompletedWords = completedLevelList8.Count;
            level8Progress.maxValue = totalNumberOfWords;
            level8Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[7])
                progressValue = "Locked";
            level8ProgressText.text = progressValue;
        }

        List<string> completedLevelList9 = currentDifficulty.level9CompletedWords;
        if (level9Progress != null && level9Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[8].words.Length;
            int numberOfCompletedWords = completedLevelList9.Count;
            level9Progress.maxValue = totalNumberOfWords;
            level9Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[8])
                progressValue = "Locked";
            level9ProgressText.text = progressValue;
        }

        List<string> completedLevelList10 = currentDifficulty.level10CompletedWords;
        if (level10Progress != null && level10Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[9].words.Length;
            int numberOfCompletedWords = completedLevelList10.Count;
            level10Progress.maxValue = totalNumberOfWords;
            level10Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[9])
                progressValue = "Locked";
            level10ProgressText.text = progressValue;
        }
        
        SetDifficultyButton();
        _audio.volume = PlayerPrefs.GetFloat(DataController.MUSIC_VOLUME);
    }

    private double CalculatePercentComplete(int count, int total)
    {
        return ((double)count / total) * 100;
    }

    private void UpdateLevelDisplay()
    {
        currentDifficulty = dataController.currentDifficulty;
        level1.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[0];
        level2.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[1];
        level3.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[2];
        level4.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[3];
        level5.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[4];
        level6.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[5];
        level7.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[6];
        level8.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[7];
        level9.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[8];
        level10.GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[9];

        List<string> completedLevelList = currentDifficulty.level1CompletedWords;
        if (level1Progress != null && level1Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[0].words.Length;
            int numberOfCompletedWords = completedLevelList.Count;
            level1Progress.maxValue = totalNumberOfWords;
            level1Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            level1ProgressText.text = progressValue;
        }

        List<string> completedLevelList2 = currentDifficulty.level2CompletedWords;
        if (level2Progress != null && level2Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[1].words.Length;
            int numberOfCompletedWords = completedLevelList2.Count;
            level2Progress.maxValue = totalNumberOfWords;
            level2Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[1])
                progressValue = "Locked";
            level2ProgressText.text = progressValue;
        }

        List<string> completedLevelList3 = currentDifficulty.level3CompletedWords;
        if (level3Progress != null && level3Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[2].words.Length;
            int numberOfCompletedWords = completedLevelList3.Count;
            level3Progress.maxValue = totalNumberOfWords;
            level3Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[2])
                progressValue = "Locked";
            level3ProgressText.text = progressValue;
        }

        List<string> completedLevelList4 = currentDifficulty.level4CompletedWords;
        if (level4Progress != null && level4Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[3].words.Length;
            int numberOfCompletedWords = completedLevelList4.Count;
            level4Progress.maxValue = totalNumberOfWords;
            level4Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[3])
                progressValue = "Locked";
            level4ProgressText.text = progressValue;
        }

        List<string> completedLevelList5 = currentDifficulty.level5CompletedWords;
        if (level5Progress != null && level5Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[4].words.Length;
            int numberOfCompletedWords = completedLevelList5.Count;
            level5Progress.maxValue = totalNumberOfWords;
            level5Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[4])
                progressValue = "Locked";
            level5ProgressText.text = progressValue;
        }

        List<string> completedLevelList6 = currentDifficulty.level6CompletedWords;
        if (level6Progress != null && level6Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[5].words.Length;
            int numberOfCompletedWords = completedLevelList6.Count;
            level6Progress.maxValue = totalNumberOfWords;
            level6Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[5])
                progressValue = "Locked";
            level6ProgressText.text = progressValue;
        }

        List<string> completedLevelList7 = currentDifficulty.level7CompletedWords;
        if (level7Progress != null && level7Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[6].words.Length;
            int numberOfCompletedWords = completedLevelList7.Count;
            level7Progress.maxValue = totalNumberOfWords;
            level7Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[6])
                progressValue = "Locked";
            level7ProgressText.text = progressValue;
        }

        List<string> completedLevelList8 = currentDifficulty.level8CompletedWords;
        if (level8Progress != null && level8Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[7].words.Length;
            int numberOfCompletedWords = completedLevelList8.Count;
            level8Progress.maxValue = totalNumberOfWords;
            level8Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[7])
                progressValue = "Locked";
            level8ProgressText.text = progressValue;
        }

        List<string> completedLevelList9 = currentDifficulty.level9CompletedWords;
        if (level9Progress != null && level9Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[8].words.Length;
            int numberOfCompletedWords = completedLevelList9.Count;
            level9Progress.maxValue = totalNumberOfWords;
            level9Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[8])
                progressValue = "Locked";
            level9ProgressText.text = progressValue;
        }

        List<string> completedLevelList10 = currentDifficulty.level10CompletedWords;
        if (level10Progress != null && level10Progress.isActiveAndEnabled)
        {
            int totalNumberOfWords = dataController.gameData.allLevelData[9].words.Length;
            int numberOfCompletedWords = completedLevelList10.Count;
            level10Progress.maxValue = totalNumberOfWords;
            level10Progress.value = numberOfCompletedWords;
            string progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
            if (!currentDifficulty.levelsUnlocked[9])
                progressValue = "Locked";
            level10ProgressText.text = progressValue;
        }
    }
    

    public void ChooseLevel1()
    {
        PlayerPrefs.SetInt("Level", 0);
        LoadGame();
    }

    public void ChooseLevel2()
    {
        PlayerPrefs.SetInt("Level", 1);
        LoadGame();
    }

    public void ChooseLevel3()
    {
        PlayerPrefs.SetInt("Level", 2);
        LoadGame();
    }

    public void ChooseLevel4()
    {
        PlayerPrefs.SetInt("Level", 3);
        LoadGame();
    }

    public void ChooseLevel5()
    {
        PlayerPrefs.SetInt("Level", 4);
        LoadGame();
    }
    public void ChooseLevel6()
    {
        PlayerPrefs.SetInt("Level", 5);
        LoadGame();
    }

    public void ChooseLevel7()
    {
        PlayerPrefs.SetInt("Level", 6);
        LoadGame();
    }

    public void ChooseLevel8()
    {
        PlayerPrefs.SetInt("Level", 7);
        LoadGame();
    }

    public void ChooseLevel9()
    {
        PlayerPrefs.SetInt("Level", 8);
        LoadGame();
    }

    public void ChooseLevel10()
    {
        PlayerPrefs.SetInt("Level",  9);
        LoadGame();
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void SetDifficultyButton()
    {
        easyButton.GetComponent<Button>().interactable = dataController.playerData.difficultyUnlocked[0];
        normalButton.GetComponent<Button>().interactable = dataController.playerData.difficultyUnlocked[1];
        hardButton.GetComponent<Button>().interactable = dataController.playerData.difficultyUnlocked[2];

        switch (dataController.playerData.difficultySelected)
        {
            case DataController.DIFFICULTY_EASY:
                SetEasyLevel();
                break;
            case DataController.DIFFICULTY_NORMAL:
                SetNormalLevel();
                break;
            case DataController.DIFFICULTY_HARD:
                SetHardLevel();
                break;
            default:
                SetEasyLevel();
                break;
        }
    }

    public void SetEasyLevel()
    {
        easyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general_selected");
        //easyButton.GetComponent<Image>().color = chosenColor;
        normalButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        //normalButton.GetComponent<Image>().color = defaultColor;
        //hardButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_EASY);

        UpdateLevelDisplay();
    }
    public void SetNormalLevel()
    {
        normalButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general_selected");
        //normalButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        //easyButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        //hardButton.GetComponent<Image>().color = defaultColor;
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_NORMAL);
        UpdateLevelDisplay();
    }
    public void SetHardLevel()
    {
        hardButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general_selected");
        //hardButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        //easyButton.GetComponent<Image>().color = defaultColor;
        normalButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        //normalButton.GetComponent<Image>().color = defaultColor;

        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_HARD);
        UpdateLevelDisplay();
    }
}