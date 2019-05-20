using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    int level;
    int experience;
    int experienceNeededToLevelUp;

    public Slider levelUpBar;
    public Text currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        level = 0;
        experience = 0;
        experienceNeededToLevelUp = 10;

        levelUpBar.value = experience;
        levelUpBar.maxValue = experienceNeededToLevelUp;

        currentLevel.text = "Level : 0";

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            experience += 2;
            levelUpBar.value = experience;
        }
        
        if (levelUpBar.value >= levelUpBar.maxValue)
        {
            IncreaseLevel();
        }
    }

    void IncreaseLevel()
    {
        experience = 0;
        levelUpBar.value = experience;

        experienceNeededToLevelUp += 10;
        levelUpBar.maxValue = experienceNeededToLevelUp;

        level += 1;
        currentLevel.text = "Level : " + level.ToString();
    }
}
