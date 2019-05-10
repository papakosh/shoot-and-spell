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
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color chosenColor = new Color32(212, 175, 55, 255);

    public string[] preschool;
    public string[] kindergarten;
    public string[] first;
    public string[] second;
    public string[] third;
    public string[] fourth;

    void Awake()
    {
        easyButton.GetComponent<Image>().color = chosenColor;
        PlayerPrefs.SetString("GameDifficulty", "EASY");
    }

    public void ABCGame()
    {
        PlayerPrefs.SetString("GameMode", "ALPHABET");
        PlayerPrefs.SetString("TargetWord", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        SceneManager.LoadScene("Game");
    }

    public void WordsGame()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        SceneManager.LoadScene("GradeLevelMenu");
    }

    public void ChoosePreschool()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "Preschool");
        PlayerPrefs.SetString("TargetWord", preschool[Random.Range(0, preschool.Length)]);
        LoadScene();
    }

    public void ChooseKindergarten()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "Kindergarten");
        PlayerPrefs.SetString("TargetWord", kindergarten[Random.Range(0, kindergarten.Length)]);
        LoadScene();
    }

    public void ChooseFirstGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "FirstGrade");
        PlayerPrefs.SetString("TargetWord", first[Random.Range(0, first.Length)]);
        LoadScene();
    }

    public void ChooseSecondGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "SecondGrade");
        PlayerPrefs.SetString("TargetWord", second[Random.Range(0, second.Length)]);
        LoadScene();
    }

    public void ChooseThirdGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "ThirdGrade");
        PlayerPrefs.SetString("TargetWord", third[Random.Range(0, third.Length)]);
        LoadScene();
    }
    public void ChooseFourthGrade()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("GradeLevel", "FourthGrade");
        PlayerPrefs.SetString("TargetWord", fourth[Random.Range(0, fourth.Length)]);
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }


    public void SetEasyLevel()
    {
        if (PlayerPrefs.GetString("GameDifficulty").Equals("EASY"))
            return;
        else
        {
            easyButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "EASY");
            normalButton.GetComponent<Image>().color = defaultColor;
            hardButton.GetComponent<Image>().color = defaultColor;
        }
    }
    public void SetNormalLevel()
    {
        if (PlayerPrefs.GetString("GameDifficulty").Equals("NORMAL"))
            return;
        else
        {
            normalButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "NORMAL");
            easyButton.GetComponent<Image>().color = defaultColor;
            hardButton.GetComponent<Image>().color = defaultColor;

        }
    }
    public void SetHardLevel()
    {
        if (PlayerPrefs.GetString("GameDifficulty").Equals("HARD"))
            return;
        else
        {
            hardButton.GetComponent<Image>().color = chosenColor;
            PlayerPrefs.SetString("GameDifficulty", "HARD");
            easyButton.GetComponent<Image>().color = defaultColor;
            normalButton.GetComponent<Image>().color = defaultColor;

        }
    }
}