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

        SetDifficultyButton();

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
        //dataController.SavePlayerSettings(DataController.DIFFICULTY_EASY);
    }
    public void SetNormalLevel()
    {
        normalButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().color = defaultColor;
        hardButton.GetComponent<Image>().color = defaultColor;
        //dataController.SavePlayerSettings(DataController.DIFFICULTY_NORMAL);
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_NORMAL);
    }
    public void SetHardLevel()
    {
        hardButton.GetComponent<Image>().color = chosenColor;
        easyButton.GetComponent<Image>().color = defaultColor;
        normalButton.GetComponent<Image>().color = defaultColor;
        //dataController.SavePlayerSettings(DataController.DIFFICULTY_HARD);
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_HARD);
    }
}