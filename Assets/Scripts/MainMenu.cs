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
        level1.GetComponent<Button>().interactable = true;
        level2.GetComponent<Button>().interactable = dataController.allLevelData[0].isComplete;
        level3.GetComponent<Button>().interactable = dataController.allLevelData[1].isComplete;
        level4.GetComponent<Button>().interactable = dataController.allLevelData[2].isComplete;
        level5.GetComponent<Button>().interactable = dataController.allLevelData[3].isComplete;
        level6.GetComponent<Button>().interactable = dataController.allLevelData[4].isComplete;
        level7.GetComponent<Button>().interactable = dataController.allLevelData[5].isComplete;
        level8.GetComponent<Button>().interactable = dataController.allLevelData[6].isComplete;
        level9.GetComponent<Button>().interactable = dataController.allLevelData[7].isComplete;
        level10.GetComponent<Button>().interactable = dataController.allLevelData[8].isComplete;

        SetDifficultyButton();

    }

    public void ChoosePreschool()
    {
        PlayerPrefs.SetInt("Level", 0);
        LoadGame();
    }

    public void ChooseKindergarten()
    {
        PlayerPrefs.SetInt("Level", 1);
        LoadGame();
    }

    public void ChooseFirstGrade()
    {
        PlayerPrefs.SetInt("Level", 2);
        LoadGame();
    }

    public void ChooseSecondGrade()
    {
        PlayerPrefs.SetInt("Level", 3);
        LoadGame();
    }

    public void ChooseThirdGrade()
    {
        PlayerPrefs.SetInt("Level", 4);
        LoadGame();
    }
    public void ChooseFourthGrade()
    {
        PlayerPrefs.SetInt("Level", 5);
        LoadGame();
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void SetDifficultyButton()
    {
        switch (PlayerPrefs.GetString("Difficulty"))
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
        }
    }

    public void SetEasyLevel()
    {
        easyButton.GetComponent<Image>().color = chosenColor;
        normalButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().color = defaultColor;
        dataController.SavePlayerSettings(DataController.DIFFICULTY_EASY);
    }
    public void SetNormalLevel()
    {
        normalButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().color = defaultColor;
        dataController.SavePlayerSettings(DataController.DIFFICULTY_NORMAL);
    }
    public void SetHardLevel()
    {
        hardButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().color = defaultColor;
        normalButton.GetComponent<Image>().color = defaultColor;
        dataController.SavePlayerSettings(DataController.DIFFICULTY_HARD);
    }
}