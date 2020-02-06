using System.Collections.Generic;

/**
 * Description: Stores a player’s progress in a difficulty, including experience points, rank,
 * levels unlocked, and a list of words completed for each level. Object IS marked System.Serializable 
 * so it can be written to a file using serialization.
 * 
 * Details:
 * ListOfLevelCompletedWords: Returns a list of words completed for a given level index
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
            case DataController.LEVEL_ONE:
                return level1CompletedWords;
            case DataController.LEVEL_TWO:
                return level2CompletedWords;
            case DataController.LEVEL_THREE:
                return level3CompletedWords;
            case DataController.LEVEL_FOUR:
                return level4CompletedWords;
            case DataController.LEVEL_FIVE:
                return level5CompletedWords;
            case DataController.LEVEL_SIX:
                return level6CompletedWords;
            case DataController.LEVEL_SEVEN:
                return level7CompletedWords;
            case DataController.LEVEL_EIGHT:
                return level8CompletedWords;
            case DataController.LEVEL_NINE:
                return level9CompletedWords;
            case DataController.LEVEL_TEN:
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