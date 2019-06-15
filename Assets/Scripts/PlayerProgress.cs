﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress
{
    public int rank; // 0=cadet, 1=?, 2=?, etc.
    public int xp;
    public List<string> level1Completed;
    public List<string> level2Completed;
    public List<string> level3Completed;
    public List<string> level4Completed;
    public List<string> level5Completed;
    public List<string> level6Completed;
    public List<string> level7Completed;
    public List<string> level8Completed;
    public List<string> level9Completed;
    public List<string> level10Completed;

    public PlayerProgress(int rank, int xp)
    {
        level1Completed = new List<string>();
        level2Completed = new List<string>();
        level3Completed = new List<string>();
        level4Completed = new List<string>();
        level5Completed = new List<string>();
        level6Completed = new List<string>();
        level7Completed = new List<string>();
        level8Completed = new List<string>();
        level9Completed = new List<string>();
        level10Completed = new List<string>();
    }
}
