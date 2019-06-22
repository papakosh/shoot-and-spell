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

    public void CallSettings()
    {
        SceneManager.LoadScene("SettingsMenu");
    }

    void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }

    private void Start()
    {
        level1.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[0];
        level2.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[1];
        level3.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[2];
        level4.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[3];
        level5.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[4];
        level6.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[5];
        level7.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[6];
        level8.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[7];
        level9.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[8];
        level10.GetComponent<Button>().interactable = dataController.playerData.levelsUnlocked[9];


        List<string> completedLevelList = dataController.getCompletedLevelList(0);
        level1Progress.maxValue = dataController.allLevelData[0].words.Length;
        level1Progress.value = completedLevelList.Count;
        string level1ProgressValue = completedLevelList.Count + " out of " + dataController.allLevelData[0].words.Length;
        level1ProgressText.text = level1ProgressValue;

        List<string> completedLevelList2 = dataController.getCompletedLevelList(1);
        level2Progress.maxValue = dataController.allLevelData[1].words.Length;
        level2Progress.value = completedLevelList2.Count;
        string level2ProgressValue = completedLevelList2.Count + " out of " + dataController.allLevelData[1].words.Length;
        if (!dataController.playerData.levelsUnlocked[1])
            level2ProgressValue = "Locked";
        level2ProgressText.text = level2ProgressValue;

        List<string> completedLevelList3 = dataController.getCompletedLevelList(2);
        level3Progress.maxValue = dataController.allLevelData[2].words.Length;
        level3Progress.value = completedLevelList3.Count;
        string level3ProgressValue = completedLevelList3.Count + " out of " + dataController.allLevelData[2].words.Length;
        if (!dataController.playerData.levelsUnlocked[2])
            level3ProgressValue = "Locked";
        level3ProgressText.text = level3ProgressValue;

        List<string> completedLevelList4 = dataController.getCompletedLevelList(3);
        level4Progress.maxValue = dataController.allLevelData[3].words.Length;
        level4Progress.value = completedLevelList4.Count;
        string level4ProgressValue = completedLevelList4.Count + " out of " + dataController.allLevelData[3].words.Length;
        if (!dataController.playerData.levelsUnlocked[3])
            level4ProgressValue = "Locked";
        level4ProgressText.text = level4ProgressValue;

        List<string> completedLevelList5 = dataController.getCompletedLevelList(4);
        level5Progress.maxValue = dataController.allLevelData[4].words.Length;
        level5Progress.value = completedLevelList5.Count;
        string level5ProgressValue = completedLevelList5.Count + " out of " + dataController.allLevelData[4].words.Length;
        if (!dataController.playerData.levelsUnlocked[4])
            level5ProgressValue = "Locked";
        level5ProgressText.text = level5ProgressValue;

        List<string> completedLevelList6 = dataController.getCompletedLevelList(5);
        level6Progress.maxValue = dataController.allLevelData[5].words.Length;
        level6Progress.value = completedLevelList6.Count;
        string level6ProgressValue = completedLevelList6.Count + " out of " + dataController.allLevelData[5].words.Length;
        if (!dataController.playerData.levelsUnlocked[5])
            level6ProgressValue = "Locked";
        level6ProgressText.text = level6ProgressValue;

        List<string> completedLevelList7 = dataController.getCompletedLevelList(6);
        level7Progress.maxValue = dataController.allLevelData[6].words.Length;
        level7Progress.value = completedLevelList7.Count;
        string level7ProgressValue = completedLevelList7.Count + " out of " + dataController.allLevelData[6].words.Length;
        if (!dataController.playerData.levelsUnlocked[6])
            level7ProgressValue = "Locked";
        level7ProgressText.text = level7ProgressValue;

        List<string> completedLevelList8 = dataController.getCompletedLevelList(7);
        level8Progress.maxValue = dataController.allLevelData[7].words.Length;
        level8Progress.value = completedLevelList8.Count;
        string level8ProgressValue = completedLevelList8.Count + " out of " + dataController.allLevelData[7].words.Length;
        if (!dataController.playerData.levelsUnlocked[7])
            level8ProgressValue = "Locked";
        level8ProgressText.text = level8ProgressValue;

        List<string> completedLevelList9 = dataController.getCompletedLevelList(8);
        level9Progress.maxValue = dataController.allLevelData[8].words.Length;
        level9Progress.value = completedLevelList9.Count;
        string level9ProgressValue = completedLevelList9.Count + " out of " + dataController.allLevelData[8].words.Length;
        if (!dataController.playerData.levelsUnlocked[8])
            level9ProgressValue = "Locked";
        level9ProgressText.text = level9ProgressValue;

        List<string> completedLevelList10 = dataController.getCompletedLevelList(9);
        level10Progress.maxValue = dataController.allLevelData[9].words.Length;
        level10Progress.value = completedLevelList10.Count;
        string level10ProgressValue = completedLevelList10.Count + " out of " + dataController.allLevelData[9].words.Length;
        if (!dataController.playerData.levelsUnlocked[9])
            level10ProgressValue = "Locked";
        level10ProgressText.text = level10ProgressValue;

        SetDifficultyButton();
    }

    private double CalculatePercentComplete(int count, int total)
    {
        return ((double)count / total) * 100;
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
        easyButton.GetComponent<Image>().color = chosenColor;
        normalButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().color = defaultColor;
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_EASY);
    }
    public void SetNormalLevel()
    {
        normalButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().color = defaultColor;
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_NORMAL);
    }
    public void SetHardLevel()
    {
        hardButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().color = defaultColor;
        normalButton.GetComponent<Image>().color = defaultColor;
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_HARD);
    }
}