﻿using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float delay;

    public float dodge;
    public float smoothing;
    public float tilt;
    public Vector2 startWait;
    public Vector2 maneuverTime;
    public Vector2 maneuverWait;
    public GameBoundary gameBoundary;

    //private float currentSpeed;
    private float targetManeuver;
    private Rigidbody rb;

    private AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        //currentSpeed = rb.velocity.z;
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        rb.velocity = transform.forward * speed;
        InvokeRepeating("Fire", delay, fireRate);
        StartCoroutine(Evade());
    }

    void Fire()
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        _audio.Play();
    }

    IEnumerator Evade()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));

        while (true)
        {
            targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x); // reverses the sign of some value, so negative becomes positive and positives become negatives (return opposite Sign value)
            yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float newManeuver = Mathf.MoveTowards(rb.velocity.x, targetManeuver, Time.deltaTime * smoothing);
        rb.velocity = new Vector3(newManeuver, 0.0f, speed);
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, gameBoundary.xMin, gameBoundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, gameBoundary.zMin, gameBoundary.zMax));

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
    }
}