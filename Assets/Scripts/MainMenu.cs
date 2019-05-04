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