using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public LevelData[] allLevelData;
    public float rankXPModifier;
    public float rankBaseXP;
    public float waveWait;
    public float spawnWait;
    public float spawnDecrement;
}
