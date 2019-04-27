using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed;
    public float tileSizeZ;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the new position of the background
        //Using Mathf.Repeat, loop the value Time.time * scrollSpeed over tileSizeZ until the value is at zero or greater than length (then start over)
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);

        //Set new position in transform to be: start positiom (likely 0) + 1 * new position
        transform.position = startPosition + Vector3.forward * newPosition;
    }
}