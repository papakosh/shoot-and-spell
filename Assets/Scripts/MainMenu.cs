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
    public GameObject preschool;
    public GameObject kindergarten;
    public GameObject firstGrade;
    public GameObject secondGrade;
    public GameObject thirdGrade;
    public GameObject fourthGrade;

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
        preschool.GetComponent<Button>().interactable = true;
        switch (PlayerPrefs.GetInt("Rank"))
        {
            case DataController.PRESCHOOL_RANK:
                kindergarten.GetComponent<Button>().interactable = false;
                firstGrade.GetComponent<Button>().interactable = false;
                secondGrade.GetComponent<Button>().interactable = false;
                thirdGrade.GetComponent<Button>().interactable = false;
                fourthGrade.GetComponent<Button>().interactable = false;
                break;
            case DataController.KINDERGARTEN_RANK:
                kindergarten.GetComponent<Button>().interactable = true;
                firstGrade.GetComponent<Button>().interactable = false;
                secondGrade.GetComponent<Button>().interactable = false;
                thirdGrade.GetComponent<Button>().interactable = false;
                fourthGrade.GetComponent<Button>().interactable = false;
                break;
            case DataController.FIRSTGRADE_RANK:
                kindergarten.GetComponent<Button>().interactable = true;
                firstGrade.GetComponent<Button>().interactable = true;
                secondGrade.GetComponent<Button>().interactable = false;
                thirdGrade.GetComponent<Button>().interactable = false;
                fourthGrade.GetComponent<Button>().interactable = false;
                break;
            case DataController.SECONDGRADE_RANK:
                kindergarten.GetComponent<Button>().interactable = true;
                firstGrade.GetComponent<Button>().interactable = true;
                secondGrade.GetComponent<Button>().interactable = true;
                thirdGrade.GetComponent<Button>().interactable = false;
                fourthGrade.GetComponent<Button>().interactable = false;
                break;
            case DataController.THIRDGRADE_RANK:
                kindergarten.GetComponent<Button>().interactable = true;
                firstGrade.GetComponent<Button>().interactable = true;
                secondGrade.GetComponent<Button>().interactable = true;
                thirdGrade.GetComponent<Button>().interactable = true;
                fourthGrade.GetComponent<Button>().interactable = false;
                break;
            case DataController.FOURTHGRADE_RANK:
                kindergarten.GetComponent<Button>().interactable = true;
                firstGrade.GetComponent<Button>().interactable = true;
                secondGrade.GetComponent<Button>().interactable = true;
                thirdGrade.GetComponent<Button>().interactable = true;
                fourthGrade.GetComponent<Button>().interactable = true;
                break;
        }

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