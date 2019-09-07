using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed;
    public float tileSizeZ;

    private Vector3 startPosition;
    private AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        _audio = gameObject.GetComponent<AudioSource>();
        _audio.volume = PlayerPrefs.GetFloat(DataController.MUSIC_VOLUME);
    }

    // Update is called once per frame
    void Update()
    {
        ScrollBackgroundToNextPosition();
    }

    private void ScrollBackgroundToNextPosition()
    {
        // Calculates next position using Mathf.Repeat where t is negative and length is added to t until the result is between zero and length
        float nextPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);

        // Move transform forward to next position
        transform.position = startPosition + Vector3.forward * nextPosition;
    }
}