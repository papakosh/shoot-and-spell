using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/**
 * Description: Regulate a player’s actions, both movement and combat, including staying within the game boundaries, 
 * single shot firing, dual shot firing, and teleportation.
 * 
 * Details - 
 * On Awake: instantiate and initialize audio sources and player speed 
 * Start:  initialize access to rigidbody component and set canFire to true
 * Update: Check for a valid combat action (teleportation, fire single shot, or fire dual shot ) and play appropriate SFX
 * FixedUpdate: Get Horizontal and Vertical movement from CrossPlatformInputManager attached to virtual joystick, and then change the velocity of the 
 * associated game object’s rigidbody to be movement multiplied by speed. The rigidbody’s position is then clamped to stay within the game 
 * boundaries along the x and z axis.
 */
[System.Serializable]
public class GameBoundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public float maxSpeed;
    public float tiltX;
    public GameBoundary gameBoundary;
    public Transform singleShotSpawn;
    public Transform[] dualShotSpawn;
    public GameObject bolt;
    public static PlayerController instance = null;
    [HideInInspector]
    public float currentSpeed;
    public bool canTeleport;
    public bool canFireDualShot;
    public bool canAbsorbDamage;
    public float currentHealth = 1;
    public float maxHealth = 1;

    private AudioSource _audio;
    private Rigidbody rb;
    private bool canFire;
    private AudioClip singleShotClip;
    private AudioClip dualShotClip;
    private AudioClip teleportUsedClip;
    private AudioClip teleportPickupClip;
    private AudioClip armorPickupClip;
    private AudioClip armorUsedClip;
    private AudioClip dualShotPickup;
    private AudioClip healthPickupClip;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        _audio = GetComponent<AudioSource>();
        singleShotClip = Resources.Load<AudioClip>("Audio/weapon_player");
        dualShotClip = Resources.Load<AudioClip>("Audio/dual_shot");
        teleportUsedClip = Resources.Load<AudioClip>("Audio/teleport_used");
        teleportPickupClip = Resources.Load<AudioClip>("Audio/teleport_pickup");
        armorPickupClip = Resources.Load<AudioClip>("Audio/armor_pickup");
        armorUsedClip = Resources.Load<AudioClip>("Audio/armor_used");
        dualShotPickup = Resources.Load<AudioClip>("Audio/dualshot_pickup");
        healthPickupClip = Resources.Load<AudioClip>("Audio/health_pickup");
        _audio.clip = singleShotClip;
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        currentSpeed = maxSpeed;
        canFire = true;
        canTeleport = false;
        canFireDualShot = false;
        canAbsorbDamage = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canTeleport && !GameController.instance.isGamePaused && hasTouchedValidLocation())
        {
           rb.position = new Vector3(Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, gameBoundary.xMin, gameBoundary.xMax),
             0.0f, Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).z, gameBoundary.zMin, gameBoundary.zMax));
            _audio.volume = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
            _audio.clip = teleportUsedClip;
            _audio.Play();
            //GameController.instance.TeleportActivated();
            canTeleport = false;
            GameController.instance.UpdateTeleportStatusIcon("DEACTIVE");
            //GameController.instance.teleportAbility = false;
            return;
        }
        else if (CrossPlatformInputManager.GetButtonDown("Jump") && canFire && !GameController.instance.isGamePaused)
        {
            if (canFireDualShot)
            {
                Instantiate(bolt, dualShotSpawn[0].position, Quaternion.Euler(0, 0, 0));
                Instantiate(bolt, dualShotSpawn[1].position, Quaternion.Euler(0, 0, 0));

                _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
                _audio.clip = dualShotClip;
                _audio.Play();
                canFireDualShot = false;
                //GameController.instance.ResetDualShot ();
                GameController.instance.UpdateDualShotStatusIcon("Deactive");
            }
            else
            {
                _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
                _audio.clip = singleShotClip;
                Instantiate(bolt, singleShotSpawn.position, Quaternion.Euler (0, 0, 0));
            }
            
            canFire = false;
            _audio.Play();
        }
        else if (CrossPlatformInputManager.GetButtonUp("Jump"))
        {
            canFire = true;
        }
    }

    // FixedUpdate is called once, zero, or several times per frame
    private void FixedUpdate()
    {
        Vector3 movement;
           float movementHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float movementVertical = CrossPlatformInputManager.GetAxis("Vertical");
            movement = new Vector3(movementHorizontal*0.40f, 0.0f, movementVertical*0.20f);
        rb.velocity = movement * currentSpeed; // sets how many units / second to move and in what direction (s) using a Vector3

        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, gameBoundary.xMin, gameBoundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, gameBoundary.zMin, gameBoundary.zMax));

        rb.rotation = Quaternion.Euler(90.0f, 0.0f, rb.velocity.x * -tiltX);
    }

    private bool hasTouchedValidLocation()
    {
        return Input.GetButtonDown("Fire1") && Camera.main.ScreenToWorldPoint(Input.mousePosition).z >= 0;
    }

    public void PickupTeleport()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        GameController.instance.UpdateTeleportStatusIcon("ACTIVE");
        canTeleport = true;
        _audio.clip = teleportPickupClip;
        _audio.Play();
    }
    public void PickupArmor()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        GameController.instance.UpdateArmorStatusIcon("ACTIVE");
        canAbsorbDamage = true;
        _audio.clip = armorPickupClip;
        _audio.Play();
    }

    public void AbsorbDamage()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        GameController.instance.UpdateArmorStatusIcon("DEACTIVE");
        _audio.clip = armorUsedClip;
        _audio.Play();
        StartCoroutine(GameController.instance.ArmorActivated());
        canAbsorbDamage = false;
    }

    public void PickupDualShot()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        //dualShotStatusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/panel_active");
        GameController.instance.UpdateDualShotStatusIcon("ACTIVE");
        canFireDualShot = true;
        _audio.clip = dualShotPickup;
        _audio.Play();
    }

    public IEnumerator DecreaseSpeed()
    {
        currentSpeed = currentSpeed / 2;
        yield return new WaitForSeconds(3.0f);
        currentSpeed = maxSpeed;
    }

    public void IncreaseHealth(float amt)
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.PICKUPS_VOLUME);
        _audio.clip = healthPickupClip;
        _audio.Play();

        if (currentHealth == maxHealth)
        {
            return;
        }
        else
        {
            currentHealth += amt;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        //StartCoroutine(HandleHealthText(3, amt, false));
        //RefreshHealthBar();
        GameController.instance.RefreshHealthBar(amt, false);
    }
    public void DecreaseHealth(float damageAmt)
    {
        if (currentHealth > 0.5f)
        {
            currentHealth -= damageAmt;
            if (currentHealth <= 0)
                GameController.instance.isDead = true;
            else
            {
                StartCoroutine(GameController.instance.BeenHit());
            }
        }
        else
        {
            currentHealth = 0;
            GameController.instance.isDead = true;
        }

        //StartCoroutine(HandleHealthText(3, damageAmt, true));
        GameController.instance.RefreshHealthBar(damageAmt, true);
    }
}