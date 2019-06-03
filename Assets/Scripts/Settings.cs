using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    public InputField skillInput;
    public InputField xpInput;

    // Start is called before the first frame update
    void Start()
    {
        skillInput.text = PlayerPrefs.GetInt("SkillLevel").ToString();
        xpInput.text = PlayerPrefs.GetInt("XP").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyChanges()
    {
        if (skillInput != null && skillInput.text != "")
        {
            PlayerPrefs.SetInt("SkillLevel", int.Parse(skillInput.text));
        }

        if (xpInput != null && xpInput.text != "")
        {
            PlayerPrefs.SetInt("XP", int.Parse(xpInput.text));
        }
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
    public void CallHome()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
