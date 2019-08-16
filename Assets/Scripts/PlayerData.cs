using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string difficultySelected;
    public bool[] difficultyUnlocked = { true, false, false };
    public LevelOfDifficulty easyDifficulty;
    public LevelOfDifficulty normalDifficulty;
    public LevelOfDifficulty hardDifficulty;
}

