/**
 * Description: Regulates how the game works.
 * 
 * Details:
 * Class-
 * Object marked System.Serializable so it can be written to a file using serialization.
 * 
 * Attributes-
 * All level data - storage of the game levels
 * XP modifier - experience points modifier to factor in word difficulty
 * Base xp - Basic XP number used in calculations
 * Wave wait - Number of seconds between waves
 * Spawn wait - Number of seconds between pieces of debris
 * Spawn wait decrement - Number of seconds to decrease wait time between pieces of debris
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
