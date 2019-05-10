﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private bool canFire;
    private Quaternion calibrationQuaternion;
    public float speed;
    public Transform shotSpawn;
    public GameObject shot;
    public Boundary boundary;
    public float tiltModifier;
    public float health;
    public static PlayerController instance = null;
    [HideInInspector]
    public bool isDead;
    private Color normalColor = Color.white;
    private Color hitColor = Color.red;
    public float flashDelay = 0.025f;
    public int timesToFlash = 3;
    public Text healthText;
    private float originalSpeed;
    private AudioSource _audio;
    private AudioSource healthPickupAudio;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();
        _audio = audioSources[0];
        healthPickupAudio = audioSources[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canFire = true;
        isDead = false;
        CalibrateAccelerometer();
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1") && canFire)
        {
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            canFire = false;
            _audio.Play();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            canFire = true;
        }
    }

    // FixedUpdate is called once, zero, or several times per frame
    private void FixedUpdate()
    {
        Vector3 movement;
        if (Application.platform == RuntimePlatform.WindowsEditor)// windows editor movement
        {
            float movementHorizontal = Input.GetAxis("Horizontal"); // returns horizontal movement strength & direction in a range of 1 to -1
            float movementVertical = Input.GetAxis("Vertical"); // returns vertical movement strength & direction in a range of 1 to -1
            movement = new Vector3(movementHorizontal, 0.0f, movementVertical);
        }
        else
        {
            //accelerometer movement
            Vector3 accelerationRaw = Input.acceleration;
            Vector3 acceleration = FixAcceleration(accelerationRaw);
            movement = new Vector3(acceleration.x, 0.0f, acceleration.y);
        }
        rb.velocity = movement * speed; // sets how many units / second to move and in what direction (s) using a Vector3

        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax));

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tiltModifier);
    }

    void CalibrateAccelerometer()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);

    }

    Vector3 FixAcceleration(Vector3 acceleration)
    {
        Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
        return fixedAcceleration;
    }

    public void IncreaseHealth(float amt)
    {
        healthPickupAudio.Play();
        if (health == 3.0f)
            return;
        else
        {
            health += amt;
            if (health > 3.0f)
                health = 3.0f;
            int healthLevel = (int)(health * 2);
            int index = 0;

            while (index < healthLevel)
            {
                GameController.instance.healthIndicator[index].SetActive(true);
                index++;
            }
        }
    }

    public void DecreaseHealth (float damageAmt)
    {
        if (health > 0.5f)
        {
            health -= damageAmt;
            StartCoroutine(BeenHit());
            int healthLevel = (int)(health * 2) - 1;
            int index = GameController.instance.healthIndicator.Length - 1;

            while (index > healthLevel)
            {
                GameController.instance.healthIndicator[index].SetActive(false);
                index--;
            }


            if (health == 0)
                isDead = true;
        }
        else
        {
            health = 0;
            GameController.instance.healthIndicator[0].SetActive(false);
            isDead = true;
        }
    }

   public IEnumerator DecreaseSpeed()
    {
        speed = speed / 2;
        yield return new WaitForSeconds(3.0f);
        speed = originalSpeed;
    }

    IEnumerator BeenHit()
    {
        for (int i = 1; i <= timesToFlash; i++)
        {
            GetComponent<Renderer>().material.color = hitColor;
            yield return new WaitForSeconds(flashDelay);
            GetComponent<Renderer>().material.color = normalColor;
            yield return new WaitForSeconds(flashDelay);
        }
    }

    public void HealthPickup()
    {
        int num = Random.Range(1, 6);
        switch (num)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                if (health < 3.0f)
                {
                    health += 0.5f;
                }
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            default:
                break;
        }
    }
}