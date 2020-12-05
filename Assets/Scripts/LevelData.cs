/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Regulates how the levels work and display to a player. 
 * 
 * Details:
 * Class-
 * The object is marked System.Serializable so it can be written to a file using serialization.
 * 
 * Attributes-
 * Name - name of the level
 * Words - an array of words to be spelled
 * Background path - path to space background
 * Completion bonus - bonus xp rewarded for completing the level
 */
[System.Serializable]
public class LevelData
{
    public string name;
    public WordData[] words;
    public string backgroundPath;
    public int completionBonus;
}
