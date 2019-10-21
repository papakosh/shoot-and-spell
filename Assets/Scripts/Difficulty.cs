using System.Collections.Generic;

/**
 * Description: Stores a player’s progress in a difficulty, including experience points, rank,
 * levels unlocked, and a list of words completed for each level.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 */
[System.Serializable]
public class Difficulty
{
    public int playerRank;
    public int playerXP;
    public List<string> level1CompletedWords;
    public List<string> level2CompletedWords;
    public List<string> level3CompletedWords;
    public List<string> level4CompletedWords;
    public List<string> level5CompletedWords;
    public List<string> level6CompletedWords;
    public List<string> level7CompletedWords;
    public List<string> level8CompletedWords;
    public List<string> level9CompletedWords;
    public List<string> level10CompletedWords;
    public bool[] levelsUnlocked = { true, false, false, false, false, false, false, false, false, false };
}