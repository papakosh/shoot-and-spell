/**
 * Description: Text and audio information for a word.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 */
[System.Serializable]
public class WordData
{
    public string text;
    public string audioPath; 	
}
