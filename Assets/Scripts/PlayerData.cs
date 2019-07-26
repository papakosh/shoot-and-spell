﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //public int rank; // 0=cadet, 1=?, 2=?, etc.
    //public int xp;
    public string difficultySelected;
    public bool[] difficultyUnlocked = { true, false, false };
    /*public List<string> level1Completed;
    public List<string> level2Completed;
    public List<string> level3Completed;
    public List<string> level4Completed;
    public List<string> level5Completed;
    public List<string> level6Completed;
    public List<string> level7Completed;
    public List<string> level8Completed;
    public List<string> level9Completed;
    public List<string> level10Completed;
    public bool[] levelsUnlocked = { true, false, false, false, false, false, false, false, false, false };
    */
    public LevelOfDifficulty easyDifficulty;
    public LevelOfDifficulty normalDifficulty;
    public LevelOfDifficulty hardDifficulty;

}

