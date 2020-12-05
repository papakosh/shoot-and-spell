/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Stores the player’s progress in the game. This includes current difficulty, 
 * what difficulties are unlocked, and progress under each difficulty.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 * 
 * Attributes-
 * Difficulty selected - The name of the difficulty selected (EASY, NORMAL, HARD)
 * Difficulty unlocked - true or false on each difficulty (true, false, false is default)
 * Easy difficulty - Store a player’s progress for easy difficulty. 
 * Normal difficulty - Store a player’s progress for normal difficulty. 
 * Hard difficulty - Store a player’s progress for hard difficulty. 
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