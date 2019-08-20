using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyChanges()
    {
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
