/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Text and audio information for a word.
 * 
 * Details: Object marked System.Serializable so it can be written to a file using serialization.
 * 
 * Attributes-
 * text - word's text value 
 * audio path - word's audio file
 */
[System.Serializable]
public class WordData
{
    public string text;
    public string audioPath; 	
}
