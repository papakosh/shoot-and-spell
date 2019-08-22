using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    public Slider musicVolSlider;
    public Slider weaponsVolSlider;
    public Slider explosionsVolSlider;
    public Slider wordsVolSlider;
    public Text musicVolText;
    public Text weaponsVolText;
    public Text explosionsVolText;
    public Text wordsVolText;
    private AudioSource _audio;
    public GameObject testStopMusicButton;
    public GameObject testStopWeaponsButton;
    public GameObject testStopExplosionsButton;
    public GameObject testStopWordsButton;
    private bool musicIsPlaying;
    private bool weaponsIsPlaying;
    private bool explosionsIsPlaying;
    private bool wordsIsPlaying;

    // Start is called before the first frame update
    void Start()
    {
        musicVolSlider.value = PlayerPrefs.GetFloat(DataController.MUSIC_VOLUME);
        musicVolText.text = Mathf.Round(musicVolSlider.value * 100f) / 100f + "";
        weaponsVolSlider.value = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        weaponsVolText.text = Mathf.Round(weaponsVolSlider.value * 100f) / 100f + "";
        explosionsVolSlider.value = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
        explosionsVolText.text = Mathf.Round (explosionsVolSlider.value * 100f) / 100f + "";
        wordsVolSlider.value = PlayerPrefs.GetFloat(DataController.WORDS_VOLUME);
        wordsVolText.text = Mathf.Round (wordsVolSlider.value * 100f) / 100f + "";
        
        _audio = GetComponent<AudioSource>();
        musicIsPlaying = false;
        weaponsIsPlaying = false;
        explosionsIsPlaying = false;
        wordsIsPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyChanges()
    {
        PlayerPrefs.SetFloat(DataController.MUSIC_VOLUME, musicVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WEAPONS_VOLUME, weaponsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.EXPLOSIONS_VOLUME, explosionsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WORDS_VOLUME, wordsVolSlider.value);
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat(DataController.MUSIC_VOLUME, musicVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WEAPONS_VOLUME, weaponsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.EXPLOSIONS_VOLUME, explosionsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WORDS_VOLUME, wordsVolSlider.value);
    }
    public void CallHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateMusicVolumeText()
    {
        musicVolText.text = Mathf.Round(musicVolSlider.value * 100f) / 100f + "";
        if (musicIsPlaying)
        {
            _audio.volume = musicVolSlider.value;
        }
    }
    public void UpdateWeaponsVolumeText()
    {
        weaponsVolText.text = Mathf.Round(weaponsVolSlider.value * 100f) / 100f + "";
        if (weaponsIsPlaying)
        {
            _audio.volume = weaponsVolSlider.value;
        }
    }

    public void UpdateExplosionsVolumeText()
    {
        explosionsVolText.text = Mathf.Round(explosionsVolSlider.value * 100f) / 100f + "";
        if (explosionsIsPlaying)
        {
            _audio.volume = explosionsVolSlider.value;
        }
    }

    public void UpdateWordsVolumeText()
    {
        wordsVolText.text = Mathf.Round(wordsVolSlider.value * 100f) / 100f + "";
        if (wordsIsPlaying)
        {
            _audio.volume = wordsVolSlider.value;
        }
    }

    public void TestMusicVolume()
    {
        // is audio playing
        // if yes, is music playing, if yes, turn off and change button text, if no 

        if (_audio.isPlaying) //
        {
            _audio.Stop();
            if (musicIsPlaying)
            {
                testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                musicIsPlaying = false;
            }
            else
            {
                if (weaponsIsPlaying)
                {
                    testStopWeaponsButton.GetComponentInChildren<Text>().text = "Test";
                    weaponsIsPlaying = false;
                }
                else if (explosionsIsPlaying)
                {
                    testStopExplosionsButton.GetComponentInChildren<Text>().text = "Test";
                    explosionsIsPlaying = false;
                }
                else
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }
                musicIsPlaying = true;
                _audio.volume = musicVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/music_background");
                _audio.Play();
                testStopMusicButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = musicVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/music_background");
            _audio.Play();
            musicIsPlaying = true;

            testStopMusicButton.GetComponentInChildren<Text>().text = "Stop";
        }
    }

    public void TestWeaponsVolume()
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();

            if (weaponsIsPlaying)
            {
                testStopWeaponsButton.GetComponentInChildren<Text>().text = "Test";
                weaponsIsPlaying = false;
            }
            else
            {
                if (musicIsPlaying)
                {
                    testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                    musicIsPlaying = false;
                }else if (explosionsIsPlaying)
                {
                    testStopExplosionsButton.GetComponentInChildren<Text>().text = "Test";
                    explosionsIsPlaying = false;
                }
                else
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }

                _audio.volume = weaponsVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/weapon_player");
                _audio.Play();
                weaponsIsPlaying = true;

                testStopWeaponsButton.GetComponentInChildren<Text>().text = "Stop";
                
            }
        }
        else
        {
            _audio.volume = weaponsVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/weapon_player");
            _audio.Play();
            weaponsIsPlaying = true;

            testStopWeaponsButton.GetComponentInChildren<Text>().text = "Stop";
        }
    }

    public void TestExplosionsVolume()
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();
            if (explosionsIsPlaying)
            {
                testStopExplosionsButton.GetComponentInChildren<Text>().text = "Test";
                explosionsIsPlaying = false;
            }
            else
            {
                if (musicIsPlaying)
                {
                    testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                    musicIsPlaying = false;
                }else if (weaponsIsPlaying)
                {
                    testStopWeaponsButton.GetComponentInChildren<Text>().text = "Test";
                    weaponsIsPlaying = false;
                }else
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }

                _audio.volume = explosionsVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/explosion_asteroid");
                _audio.Play();
                explosionsIsPlaying = true;
                testStopExplosionsButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = explosionsVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/explosion_asteroid");
            _audio.Play();
            explosionsIsPlaying = true;

            testStopExplosionsButton.GetComponentInChildren<Text>().text = "Stop";
        }
    }

    public void TestWordsVolume()
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();
            if (wordsIsPlaying)
            {
                testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                wordsIsPlaying = false;
            }
            else
            {
                if (musicIsPlaying)
                {
                    testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                    musicIsPlaying = false;
                }else if (weaponsIsPlaying)
                {
                    testStopWeaponsButton.GetComponentInChildren<Text>().text = "Test";
                    weaponsIsPlaying = false;
                }
                else
                {
                    testStopExplosionsButton.GetComponentInChildren<Text>().text = "Test";
                    explosionsIsPlaying = false;
                }
                _audio.volume = wordsVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/level 1/away");
                _audio.Play();
                wordsIsPlaying = true;

                testStopWordsButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = wordsVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/level 1/away");
            _audio.Play();
            wordsIsPlaying = true;

            testStopWordsButton.GetComponentInChildren<Text>().text = "Stop";
        }
    }
}
