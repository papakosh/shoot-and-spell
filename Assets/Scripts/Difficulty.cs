using System.Collections.Generic;

/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Store a player’s progress for a given difficulty. 
 * 
 * Details:
 * Attributes-
 * Name - Name of the difficulty (easy, normal or hard)
 * Player rank - Player's rank under the difficulty
 * Player xp - Player's experience points under the difficulty
 * Level 1 completed words - List of words completed by the player for level 1 under the difficulty
 * Level 2 completed words - List of words completed by the player for level 2 under the difficulty
 * Level 3 completed words - List of words completed by the player for level 3 under the difficulty
 * Level 4 completed words - List of words completed by the player for level 4 under the difficulty
 * Level 5 completed words - List of words completed by the player for level 5 under the difficulty
 * Level 6 completed words - List of words completed by the player for level 6 under the difficulty
 * Level 7 completed words - List of words completed by the player for level 7 under the difficulty
 * Level 8 completed words - List of words completed by the player for level 8 under the difficulty
 * Level 9 completed words - List of words completed by the player for level 9 under the difficulty
 * Level 10 completed words - List of words completed by the player for level 10 under the difficulty
 * Levels Unlocked - true/false for levels unlocked by the player under the difficulty. By default, level 1 is unlocked.
 * 
 * Methods-
 * ListOfLevelCompletedWords: Return a list of words completed by the player given a level index
 * AddToListOfLevelCompletedWords: Add a word to the list of completed words for a level if not there
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