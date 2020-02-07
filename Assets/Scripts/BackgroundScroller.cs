using UnityEngine;
/**
 * Description: Creates a parallax effect to mimic a ship flying through space 
 * by having the background always moving
 * 
 * Details: 
 * Attributes-
 * Scroll speed - How fast to scroll and in which direction
 * Background height on Z - How tall the background image is on the z-axis
 * Origin - Vector 3 position values for the origin of the background image
 * _audio - Audio source for the background music
 * 
 * Methods-
 * Start - Initialize origin, audio source, and background music volume.
 * Update - Scroll down the background image to achieve the parallax effect
 * Scrolldown - Calculate the background's next position and then move its transform there.
 * The next positions is calculated by using the Mathf.Repeat function where t is negative 
 * (to scroll downward) and a length is added to t until the result is between zero and length
 */
public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed;
    public float backgroundHeightOnZ;

    private Vector3 origin;
    private AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        _audio = gameObject.GetComponent<AudioSource>();
        _audio.volume = PlayerPrefs.GetFloat(DataController.MUSIC_VOLUME);
    }

    // Update is called once per frame
    void Update()
    {
        ScrollDown();
    }

    private void ScrollDown()
    {
        float nextPosition = Mathf.Repeat(Time.time * scrollSpeed, backgroundHeightOnZ);
        transform.position = origin + Vector3.forward * nextPosition;
    }
}