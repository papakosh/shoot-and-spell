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
    public string name;
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
    public List<string> ListOfLevelCompletedWords(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0:
                return level1CompletedWords;
            case 1:
                return level2CompletedWords;
            case 2:
                return level3CompletedWords;
            case 3:
                return level4CompletedWords;
            case 4:
                return level5CompletedWords;
            case 5:
                return level6CompletedWords;
            case 6:
                return level7CompletedWords;
            case 7:
                return level8CompletedWords;
            case 8:
                return level9CompletedWords;
            case 9:
                return level10CompletedWords;
            default:
                return null;
        }
    }
    public void AddToListOfLevelCompletedWords(int levelIndex, string word)
    {
        List<string> completedWords = ListOfLevelCompletedWords(levelIndex);
        if (!completedWords.Contains(word))
            completedWords.Add(word);
    }
}