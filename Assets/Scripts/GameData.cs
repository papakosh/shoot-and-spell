/**
 * Description: Regulates how the game works. This includes an experience points modifier 
 * to factor in word difficulty, wait times for spawning hazards, and managing level data.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 */
[System.Serializable]
public class GameData
{
    public LevelData[] allLevelData;
    public float xpModifier;
    public float baseXP;
    public float waveWait;
    public float spawnWait;
    public float spawnWaitDecrement;
}
