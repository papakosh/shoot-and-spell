using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Regulates the game settings, including volume for music, weapons, explosions, 
 * and voices; joystick left-right position; and resetting everything to default.
 * 
 * Details: 
 * Attributes-
 * Music Vol Slider - UI object to control music volume value
 * Weapons Vol Slider - UI object to control weapons volume value
 * Explosions Vol Slider - UI object to control explosions volume value
 * Voices Vol Slider - UI object to control voices volume value
 * Music Vol Text - UI object to display music volume value
 * Weapons Vol Text - UI object to display weapons volume value
 * Explosions Vol Text - UI object to display explosions volume value
 * Voices Vol Text - UI object to display voices volume value
 * Test Stop Music button - Button for play / stop music in settings
 * Test Stop Weapons button - Button for play / stop weapons in settings
 * Test Stop Explosions  button - Button for play / stop explosions in settings
 * Test Stop Voices button - Button for play / stop voices in settings
 * Left Hand Control toggle - Radio button for selecting left handed controls
 * Right Hand Control toggle - Radio button for selecting right handed controls
 * 
 * Mathods -
 * ApplyChanges - Save setting changes to player prefs
 * ResetAll - Reset Player Prefs and put settings back to default values
 * LoadMainMenu - Return to main menu
 * RefreshVolume - Refresh text to show changes to volume and update audio.volume to new value, if playing
 * Test(music/weapons/explosions/voices)Volume - If audio playing, stop, then check to see which 
 * volume was playing (music/weapons/explosions/voices) and set test/stop button text to test, 
 * else set (music/weapons/explosions/voices) test/stop button text to stop and start playing.
 * 
 */
public class SettingsController : MonoBehaviour
{
    public Slider musicVolSlider;
    public Slider weaponsVolSlider;
    public Slider explosionsVolSlider;
    public Slider voicesVolSlider;

    public Text musicVolText;
    public Text weaponsVolText;
    public Text explosionsVolText;
    public Text voicesVolText;

    public GameObject testStopMusicButton;
    public GameObject testStopWeaponsButton;
    public GameObject testStopExplosionsButton;
    public GameObject testStopVoicesButton;

    public Toggle leftHandControlToggle;
    public Toggle rightHandControlToggle;

    private AudioSource _audio;
    private bool musicIsPlaying;
    private bool weaponsIsPlaying;
    private bool explosionsIsPlaying;
    private bool voicesIsPlaying;

    public void ApplyChanges()
    {
        PlayerPrefs.SetFloat(DataController.MUSIC_VOLUME, musicVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WEAPONS_VOLUME, weaponsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.EXPLOSIONS_VOLUME, explosionsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.VOICES_VOLUME, voicesVolSlider.value);
       
        if (leftHandControlToggle.isOn) PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_LEFT);
        else PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_RIGHT);
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat(DataController.MUSIC_VOLUME, musicVolSlider.value);
        PlayerPrefs.SetFloat(DataController.WEAPONS_VOLUME, weaponsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.EXPLOSIONS_VOLUME, explosionsVolSlider.value);
        PlayerPrefs.SetFloat(DataController.VOICES_VOLUME, voicesVolSlider.value);
       
        if (leftHandControlToggle.isOn) PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_LEFT);
        else PlayerPrefs.SetString(DataController.JOYSTICK_CONTROL, DataController.JOYSTICK_CONTROL_RIGHT);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(DataController.MAIN_MENU_SCENE);
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

    public void RefreshVoicesVolume()
    {
        voicesVolText.text = FormatPercentage(voicesVolSlider.value);
        if (voicesIsPlaying) _audio.volume = voicesVolSlider.value;
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
            else if (voicesIsPlaying)
            {
                testStopVoicesButton.GetComponentInChildren<Text>().text = "Test";
                voicesIsPlaying = false;
            }
        }
            musicIsPlaying = true;
            _audio.volume = musicVolSlider.value;
            _audio.clip = Resources.Load<AudioClip>("Audio/Settings/background_music");
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
            else if (voicesIsPlaying)
            {
                testStopVoicesButton.GetComponentInChildren<Text>().text = "Test";
                voicesIsPlaying = false;
            }
        }
        weaponsIsPlaying = true;
        _audio.volume = weaponsVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/Player/weapon_player");
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
            else if (voicesIsPlaying)
            {
                testStopVoicesButton.GetComponentInChildren<Text>().text = "Test";
               voicesIsPlaying = false;
            }
        }
        explosionsIsPlaying = true;
        _audio.volume = explosionsVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/Settings/asteroid_explosion");
        _audio.Play();
        testStopExplosionsButton.GetComponentInChildren<Text>().text = "Stop";
    }

    public void TestVoicesVolume()
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();
            if (voicesIsPlaying)
            {
                testStopVoicesButton.GetComponentInChildren<Text>().text = "Test";
                voicesIsPlaying = false;
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
        }
        voicesIsPlaying = true;
        _audio.volume = voicesVolSlider.value;
        _audio.clip = Resources.Load<AudioClip>("Audio/Words/Level 1/balloon");
        _audio.Play();
        testStopVoicesButton.GetComponentInChildren<Text>().text = "Stop";
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
        voicesVolSlider.value = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        voicesVolText.text = FormatPercentage(voicesVolSlider.value);
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
        voicesIsPlaying = false;
    }

    private string FormatPercentage(float value)
    {
        return Mathf.Round(value * 100f) + "%";
    }
}