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
    public Slider pickupsVolSlider;
    public Text musicVolText;
    public Text weaponsVolText;
    public Text explosionsVolText;
    public Text wordsVolText;
    public Text pickupsVolText;
    private AudioSource _audio;
    public GameObject testStopMusicButton;
    public GameObject testStopWeaponsButton;
    public GameObject testStopExplosionsButton;
    public GameObject testStopWordsButton;
    public GameObject testStopPickupsButton;
    private bool musicIsPlaying;
    private bool weaponsIsPlaying;
    private bool explosionsIsPlaying;
    private bool wordsIsPlaying;
    private bool pickupsIsPlaying;
    public Toggle leftHandControlToggle;
    public Toggle rightHandControlToggle;

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
        pickupsVolSlider.value = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        pickupsVolText.text = Mathf.Round(pickupsVolSlider.value * 100f) / 100f + "";
        if (PlayerPrefs.GetString(DataController.JOYSTICK_CONTROL).Equals(DataController.JOYSTICK_CONTROL_LEFT))
            leftHandControlToggle.isOn = true;
        else
            rightHandControlToggle.isOn = true;

        _audio = GetComponent<AudioSource>();
        musicIsPlaying = false;
        weaponsIsPlaying = false;
        explosionsIsPlaying = false;
        wordsIsPlaying = false;
        pickupsIsPlaying = false;
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
        PlayerPrefs.SetFloat(DataController.PICKUPS_VOLUME, pickupsVolSlider.value);
        if (leftHandControlToggle.isOn)
            PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_LEFT);
        else
            PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_RIGHT);
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat(DataController.MUSIC_VOLUME, musicVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WEAPONS_VOLUME, weaponsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.EXPLOSIONS_VOLUME, explosionsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WORDS_VOLUME, wordsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.PICKUPS_VOLUME, pickupsVolSlider.value);
        if (leftHandControlToggle.isOn)
            PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_LEFT);
        else
            PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_RIGHT);

        // need to do reset back to defaults

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

    public void UpdatePickupsVolumeText()
    {
        pickupsVolText.text = Mathf.Round(pickupsVolSlider.value * 100f) / 100f + "";
        if (pickupsIsPlaying)
        {
            _audio.volume = pickupsVolSlider.value;
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
                else if (wordsIsPlaying)
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }
                else
                {
                    testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                    pickupsIsPlaying = false;
                }
                musicIsPlaying = true;
                _audio.volume = musicVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/background_music");
                _audio.Play();
                testStopMusicButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = musicVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/background_music");
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
                else if (wordsIsPlaying)
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }
                else
                {
                    testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                    pickupsIsPlaying = false;
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
                }else if  (wordsIsPlaying)
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }
                else
                {
                    testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                    pickupsIsPlaying = false;
                }

                _audio.volume = explosionsVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/asteroid_explosion");
                _audio.Play();
                explosionsIsPlaying = true;
                testStopExplosionsButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = explosionsVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/asteroid_explosion");
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
                else if (wordsIsPlaying)
                {
                    testStopExplosionsButton.GetComponentInChildren<Text>().text = "Test";
                    explosionsIsPlaying = false;
                }
                else
                {
                    testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                    pickupsIsPlaying = false;
                }
                _audio.volume = wordsVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/level 1/balloon");
                _audio.Play();
                wordsIsPlaying = true;

                testStopWordsButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = wordsVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/level 1/balloon");
            _audio.Play();
            wordsIsPlaying = true;

            testStopWordsButton.GetComponentInChildren<Text>().text = "Stop";
        }
    }

    public void TestPickupsVolume()
    {
        if (_audio.isPlaying) {
            _audio.Stop();
            if (pickupsIsPlaying)
            {
                testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                pickupsIsPlaying = false;
            }
            else
            {
                if (musicIsPlaying)
                {
                    testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                    musicIsPlaying = false;
                } else if (weaponsIsPlaying)
                {
                    testStopWeaponsButton.GetComponentInChildren<Text>().text = "Test";
                    weaponsIsPlaying = false;
                } else if (explosionsIsPlaying)
                {
                    testStopExplosionsButton.GetComponentInChildren<Text>().text = "Test";
                    explosionsIsPlaying = false;
                } else
                {
                    testStopWordsButton.GetComponentInChildren<Text>().text = "Test";
                    wordsIsPlaying = false;
                }

                _audio.volume = pickupsVolSlider.value;
                _audio.clip = Resources.Load<AudioClip>("Audio/health_pickup");
                _audio.Play();
                pickupsIsPlaying = true;

                testStopPickupsButton.GetComponentInChildren<Text>().text = "Stop";
            }
        }
        else
        {
            _audio.volume = pickupsVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/health_pickup");
            _audio.Play();
            pickupsIsPlaying = true;

            testStopPickupsButton.GetComponentInChildren<Text>().text = "Stop";
        }
    }
}
