/**
 * Description: Stores the player’s progress in the game. This includes current difficulty, 
 * what difficulties are unlocked, and progress under each difficulty.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 */
[System.Serializable]
public class PlayerData
{
    public string difficultySelected;
    public bool[] difficultyUnlocked = { true, false, false };
    public Difficulty easyDifficulty;
    public Difficulty normalDifficulty;
    public Difficulty hardDifficulty;
}