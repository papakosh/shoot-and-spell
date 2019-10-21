/**
 * Description: Regulates how levels work and display to a player. This includes the name 
 * of the level, the words, the space background, and the bonus experience points awarded 
 * for completing a level.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 */
[System.Serializable]
public class LevelData
{
    public string name;
    public WordData[] words;
    public string backgroundPath;
    public int completionBonus;
}
