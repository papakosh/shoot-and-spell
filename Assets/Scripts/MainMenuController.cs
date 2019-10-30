using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Description: Regulates the display of the main menu.
 * 
 * Details:
 * LoadSettings : Go to the Settings screen
 * LoadInfo: Go to the Info screen
 * SelectLevels: Load Game screen for levels 1 to 10
 * SelectEasyDifficulty: Mark easy button selected and other two deselected, update player difficulty
 * and refresh level progress for easy difficulty.
 * SelectNormalDifficulty: Mark normal button selected and other two deselected, update player difficulty
 * and refresh level progress for normal difficulty.
 * SelectHardDifficulty: Mark hard button selected and other two deselected, update player difficulty
 * and refresh level progress for hard difficulty.
 * Awake: Find data controller instance and initialize audio component
 * Start: Reset temporary data before new session, refresh level progress for selected difficulty and set 
 * state of difficulty buttons (selected or deselected).
 * ResetTempDataBeforeNewSession: Delete keys from PlayerPrefs before new session so previous values
 * on health, player streak, dual shot, armor, and  teleport don't exist should the game have not exited 
 * normally in the previous session.
 * RefreshLevelProgress: Refresh the progress of each level (will show locked or the number of words 
 * completed out of total words for the level)
 * LoadGame: Set the level index (0 to 9) into PlayerPrefs and load game screen
 * SetDifficultyButtonState: Set the state of the difficulty buttons (locked or unlocked) and which one is selected 
 */
public class MainMenuController : MonoBehaviour
{
    public GameObject easyButton;
    public GameObject normalButton;
    public GameObject hardButton;
    public GameObject[] levelButtons;
    public Slider[] levelProgressSliders;
    public Text[] levelProgressText;

    private DataController dataController;
    private Difficulty currentDifficulty;
    private AudioSource _audio;

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void LoadInfo()
    {
        SceneManager.LoadScene("Info");
    }

    public void SelectLevel1()
    {
        LoadGame(0);
    }

    public void SelectLevel2()
    {
        LoadGame(1);
    }

    public void SelectLevel3()
    {
        LoadGame(2);
    }

    public void SelectLevel4()
    {
        LoadGame(3);
    }

    public void SelectLevel5()
    {
        LoadGame(4);
    }
    public void SelectLevel6()
    {
        LoadGame(5);
    }

    public void SelectLevel7()
    {
        LoadGame(6);
    }

    public void SelectLevel8()
    {
        LoadGame(7);
    }

    public void SelectLevel9()
    {
        LoadGame(8);
    }

    public void SelectLevel10()
    {
        LoadGame(9);
    }

    public void SelectEasyDifficulty()
    {
        easyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general_selected");
        normalButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        hardButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_EASY);

        currentDifficulty = dataController.currentDifficulty;
        RefreshLevelProgress();
    }
    public void SelectNormalDifficulty()
    {
        normalButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general_selected");
        easyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        hardButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_NORMAL);

        currentDifficulty = dataController.currentDifficulty;
        RefreshLevelProgress();
    }
    public void SelectHardDifficulty()
    {
        hardButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general_selected");
        easyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        normalButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/button_general");
        dataController.UpdatePlayerDifficulty(DataController.DIFFICULTY_HARD);

        currentDifficulty = dataController.currentDifficulty;
        RefreshLevelProgress();
    }

    void Awake()
    {
        dataController = FindObjectOfType<DataController>();
        _audio = GetComponent<AudioSource>();
        _audio.volume = PlayerPrefs.GetFloat(DataController.MUSIC_VOLUME);
    }

    private void Start()
    {
        currentDifficulty = dataController.currentDifficulty;
        ResetTempDataBeforeNewSession();

        RefreshLevelProgress();

        SetDifficultyButtonState();
    }
    private void ResetTempDataBeforeNewSession()
    {
        PlayerPrefs.DeleteKey("PreviousRoundHealth");
        PlayerPrefs.DeleteKey("PlayerStreak");
        PlayerPrefs.DeleteKey("DualShot");
        PlayerPrefs.DeleteKey("Armor");
        PlayerPrefs.DeleteKey("Teleport");
    }

    private void RefreshLevelProgress()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].GetComponent<Button>().interactable = currentDifficulty.levelsUnlocked[i];
        }

        for (int i = 0; i < levelProgressSliders.Length; i++)
        {
            List<string> listOfLevelCompletedWords = currentDifficulty.ListOfLevelCompletedWords(i);
            if (levelProgressSliders[i] != null && levelProgressSliders[i].isActiveAndEnabled)
            {
                int totalNumberOfWords = dataController.gameData.allLevelData[i].words.Length;
                int numberOfCompletedWords = listOfLevelCompletedWords.Count;
                levelProgressSliders[i].maxValue = totalNumberOfWords;
                levelProgressSliders[i].value = numberOfCompletedWords;
                string progressValue = "Locked";
                if (currentDifficulty.levelsUnlocked[i])
                {
                    progressValue = numberOfCompletedWords + " out of " + totalNumberOfWords;
                }
                levelProgressText[i].text = progressValue;
            }
        }
    }
    
    private void LoadGame(int levelIndex)
    {
        PlayerPrefs.SetInt("Level", levelIndex);
        SceneManager.LoadScene("Game");
    }

    private void SetDifficultyButtonState()
    {
        easyButton.GetComponent<Button>().interactable = dataController.playerData.difficultyUnlocked[0];
        normalButton.GetComponent<Button>().interactable = dataController.playerData.difficultyUnlocked[1];
        hardButton.GetComponent<Button>().interactable = dataController.playerData.difficultyUnlocked[2];

        switch (dataController.playerData.difficultySelected)
        {
            case DataController.DIFFICULTY_EASY:
                SelectEasyDifficulty();
                break;
            case DataController.DIFFICULTY_NORMAL:
                SelectNormalDifficulty();
                break;
            case DataController.DIFFICULTY_HARD:
                SelectHardDifficulty();
                break;
            default:
                SelectEasyDifficulty();
                break;
        }
    }
}