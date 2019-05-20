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

    void Awake()
    {

        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("SkillLevel", GameController.PRESCHOOL_SKILLEVEL);
            PlayerPrefs.SetInt("XP", 0);
            // Enable/Disable for skill level
            preschool.GetComponent<Button>().interactable = true;
            kindergarten.GetComponent<Button>().interactable = false;
            firstGrade.GetComponent<Button>().interactable = false;
            secondGrade.GetComponent<Button>().interactable = false;
            thirdGrade.GetComponent<Button>().interactable = false;
            fourthGrade.GetComponent<Button>().interactable = false;
        }
        else{
            preschool.GetComponent<Button>().interactable = true;
            switch (PlayerPrefs.GetInt("SkillLevel"))
            {
                case GameController.PRESCHOOL_SKILLEVEL:
                    kindergarten.GetComponent<Button>().interactable = false;
                    firstGrade.GetComponent<Button>().interactable = false;
                    secondGrade.GetComponent<Button>().interactable = false;
                    thirdGrade.GetComponent<Button>().interactable = false;
                    fourthGrade.GetComponent<Button>().interactable = false;
                    break;
                case GameController.KINDERGARTEN_SKILLEVEL:
                    kindergarten.GetComponent<Button>().interactable = true;
                    firstGrade.GetComponent<Button>().interactable = false;
                    secondGrade.GetComponent<Button>().interactable = false;
                    thirdGrade.GetComponent<Button>().interactable = false;
                    fourthGrade.GetComponent<Button>().interactable = false;
                    break;
                case GameController.FIRSTGRADE_SKILLEVEL:
                    kindergarten.GetComponent<Button>().interactable = true;
                    firstGrade.GetComponent<Button>().interactable = true;
                    secondGrade.GetComponent<Button>().interactable = false;
                    thirdGrade.GetComponent<Button>().interactable = false;
                    fourthGrade.GetComponent<Button>().interactable = false;
                    break;
                case GameController.SECONDGRADE_SKILLEVEL:
                    kindergarten.GetComponent<Button>().interactable = true;
                    firstGrade.GetComponent<Button>().interactable = true;
                    secondGrade.GetComponent<Button>().interactable = true;
                    thirdGrade.GetComponent<Button>().interactable = false;
                    fourthGrade.GetComponent<Button>().interactable = false;
                    break;
                case GameController.THIRDGRADE_SKILLEVEL:
                    kindergarten.GetComponent<Button>().interactable = true;
                    firstGrade.GetComponent<Button>().interactable = true;
                    secondGrade.GetComponent<Button>().interactable = true;
                    thirdGrade.GetComponent<Button>().interactable = true;
                    fourthGrade.GetComponent<Button>().interactable = false;
                    break;
                case GameController.FOURTHGRADE_SKILLEVEL:
                    kindergarten.GetComponent<Button>().interactable = true;
                    firstGrade.GetComponent<Button>().interactable = true;
                    secondGrade.GetComponent<Button>().interactable = true;
                    thirdGrade.GetComponent<Button>().interactable = true;
                    fourthGrade.GetComponent<Button>().interactable = true;
                    break;
            }
        }

        SetDifficultyButton();

    }

    public void ChoosePreschool()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "Preschool");
        LoadScene();
    }

    public void ChooseKindergarten()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "Kindergarten");
        LoadScene();
    }

    public void ChooseFirstGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "FirstGrade");
        LoadScene();
    }

    public void ChooseSecondGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "SecondGrade");
        LoadScene();
    }

    public void ChooseThirdGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "ThirdGrade");
        LoadScene();
    }
    public void ChooseFourthGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "FourthGrade");
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }

    private void SetDifficultyButton()
    {
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            easyButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "EASY");
            normalButton.GetComponent<Image>().color = defaultColor;
            hardButton.GetComponent<Image>().color = defaultColor;
        }
        else if (PlayerPrefs.GetString("GameDifficulty").Equals("EASY"))
        {
            SetEasyLevel();
        }else if (PlayerPrefs.GetString("GameDifficulty").Equals("NORMAL"))
        {
            SetNormalLevel();
        }
        else
        {
            SetHardLevel();
        }
    }

    public void SetEasyLevel()
    {
          easyButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "EASY");
            normalButton.GetComponent<Image>().color = defaultColor;
            hardButton.GetComponent<Image>().color = defaultColor;
        
    }
    public void SetNormalLevel()
    {
       
            normalButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "NORMAL");
            easyButton.GetComponent<Image>().color = defaultColor;
            hardButton.GetComponent<Image>().color = defaultColor;

        
    }
    public void SetHardLevel()
    {
            hardButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "HARD");
            easyButton.GetComponent<Image>().color = defaultColor;
            normalButton.GetComponent<Image>().color = defaultColor;
    }
}