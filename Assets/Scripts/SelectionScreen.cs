﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseAlphabet()
    {
        PlayerPrefs.SetString("GameMode", "ALPHABET");
        PlayerPrefs.SetString("TargetWord", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        SceneManager.LoadScene("Scene1");
    }

    public void ChooseWord()
    {
        PlayerPrefs.SetString("GameMode", "WORD");
        PlayerPrefs.SetString("TargetWord", "ACCIDENT");
        SceneManager.LoadScene("Scene1");
    }
}
