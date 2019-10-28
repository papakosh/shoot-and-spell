using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/**
 * Description: Regulates the game settings, including volume for music, weapons, explosions, 
 * pickups and words; joystick left-right position; and resetting everything to default.
 * 
 * Details: 
 * ApplyChanges - Save setting changes to player prefs
 * ResetAll - Reset Player Prefs and put settings back to default values
 * LoadMainMenu: Return to main menu
 * RefreshVolume - Refresh text to show changes to volume and update audio.volume to new value, if playing
 * Test(music/weapons/explosions/words/pickups)Volume - If audio playing, stop, then check to see which 
 * volume was playing (music/weapons/explosions/words/pickups) and set test/stop button text to test, 
 * else set (music/weapons/explosions/words/pickups) test/stop button text to stop and start playing.
 * 
 */
public class SettingsController : MonoBehaviour
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
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RefreshMusicVolume()
    {
        musicVolText.text = FormatPercentage(musicVolSlider.value);
        if (musicIsPlaying) _audio.volume = musicVolSlider.value;
    }
    public void RefreshWeaponsVolume()
    {
        weaponsVolText.text = FormatPercentage(weaponsVolSlider.value);
        if (weaponsIsPlaying) _audio.volume = weaponsVolSlider.value;
    }

    public void RefreshExplosionsVolume()
    {
        explosionsVolText.text = FormatPercentage(explosionsVolSlider.value);
        if (explosionsIsPlaying) _audio.volume = explosionsVolSlider.value;
    }

    public void RefreshWordsVolume()
    {
        wordsVolText.text = FormatPercentage(wordsVolSlider.value);
        if (wordsIsPlaying) _audio.volume = wordsVolSlider.value;
    }

    public void RefreshPickupsVolume()
    {
        pickupsVolText.text = FormatPercentage(pickupsVolSlider.value);
        if (pickupsIsPlaying) _audio.volume = pickupsVolSlider.value;
    }

    public void TestMusicVolume()
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();
            if (musicIsPlaying)
            {
                testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                musicIsPlaying = false;
                return;
            }
            else if (weaponsIsPlaying)
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
            
        }
            musicIsPlaying = true;
            _audio.volume = musicVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/background_music");
            _audio.Play();
            testStopMusicButton.GetComponentInChildren<Text>().text = "Stop";
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
                return;
            }
            else if (musicIsPlaying)
            {
                testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                musicIsPlaying = false;
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
        }
        weaponsIsPlaying = true;
        _audio.volume = weaponsVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/weapon_player");
        _audio.Play();
        testStopWeaponsButton.GetComponentInChildren<Text>().text = "Stop";
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
                return;
            }
            else if (musicIsPlaying)
            {
                testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                musicIsPlaying = false;
            }
            else if (weaponsIsPlaying)
            {
                testStopWeaponsButton.GetComponentInChildren<Text>().text = "Test";
                weaponsIsPlaying = false;
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
        }
        explosionsIsPlaying = true;
        _audio.volume = explosionsVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/asteroid_explosion");
        _audio.Play();
        testStopExplosionsButton.GetComponentInChildren<Text>().text = "Stop";
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
                return;
            }
            else if (musicIsPlaying)
            {
                testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                musicIsPlaying = false;
            }
            else if (weaponsIsPlaying)
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
                testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                pickupsIsPlaying = false;
            }
        }
        wordsIsPlaying = true;
        _audio.volume = wordsVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/level 1/balloon");
        _audio.Play();
        testStopWordsButton.GetComponentInChildren<Text>().text = "Stop";
    }

    public void TestPickupsVolume()
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();
            if (pickupsIsPlaying)
            {
                testStopPickupsButton.GetComponentInChildren<Text>().text = "Test";
                pickupsIsPlaying = false;
                return;
            }
            else if (musicIsPlaying)
            {
                testStopMusicButton.GetComponentInChildren<Text>().text = "Test";
                musicIsPlaying = false;
            }
            else if (weaponsIsPlaying)
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
        }
        pickupsIsPlaying = true;
        _audio.volume = pickupsVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/health_pickup");
        _audio.Play();
        testStopPickupsButton.GetComponentInChildren<Text>().text = "Stop";
    }

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        InitializeVolumeSliders();
        InitializeJoystickControlOptions();
        IntializeIsPlayingStatuses();
    }

    private void InitializeVolumeSliders()
    {
        musicVolSlider.value = PlayerPrefs.GetFloat(DataController.MUSIC_VOLUME);
        musicVolText.text = FormatPercentage(musicVolSlider.value);
        weaponsVolSlider.value = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        weaponsVolText.text = FormatPercentage(weaponsVolSlider.value);
        explosionsVolSlider.value = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
        explosionsVolText.text = FormatPercentage(explosionsVolSlider.value);
        wordsVolSlider.value = PlayerPrefs.GetFloat(DataController.WORDS_VOLUME);
        wordsVolText.text = FormatPercentage(wordsVolSlider.value);
        pickupsVolSlider.value = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        pickupsVolText.text = FormatPercentage(pickupsVolSlider.value);

    }

    private void InitializeJoystickControlOptions()
    {
        if (PlayerPrefs.GetString(DataController.JOYSTICK_CONTROL).Equals(DataController.JOYSTICK_CONTROL_LEFT))
            leftHandControlToggle.isOn = true;
        else
            rightHandControlToggle.isOn = true;
    }

    private void IntializeIsPlayingStatuses()
    {
        musicIsPlaying = false;
        weaponsIsPlaying = false;
        explosionsIsPlaying = false;
        wordsIsPlaying = false;
        pickupsIsPlaying = false;
    }

    private string FormatPercentage(float value)
    {
        return Mathf.Round(value * 100f) + "%";
    }
}