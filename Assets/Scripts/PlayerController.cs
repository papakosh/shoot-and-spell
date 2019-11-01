using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/**
 * Description: Regulate a player’s stats and actions, both movement and combat, including pick ups, taking damage, 
 * absorbing damage, decreasing speed, moving within the game boundaries, single shot firing, dual shot firing, and 
 * teleportation.
 * 
 * Details - 
 * PickupTeleport - Play sfx, set skill to true and update game controller
 * PickupArmor - Play sfx, set skill to true and update game controller
 * PickupDualShot - Play sfx, set skill to true and update game controller
 * PickupHealth - Play sfx, increase health and update game controller
 * DecreaseSpeed - Play sfx, reduce speed
 * NormalizeSpeed - Set speed to maximum
 * AbsorbDamage - Play sfx,  play vfx, set skill to false and update game controller
 * TakeDamage - Play sfx,  play vfx, and update game controller
 * On Awake: instantiate and initialize audio sources, player speed, and skills (fire, dualshot, teleport, absorb damage)
 * Start:  initialize access to rigidbody component
 * Update: Check for a valid combat action (teleport, fire single shot, or fire dual shot ), do action, play appropriate 
 * SFX and update game controller, if needed
 * FixedUpdate: Get Horizontal and Vertical movement from virtual joystick, and then change the velocity of the 
 * associated game object’s rigidbody to be movement multiplied by speed. The rigidbody’s position is then clamped to stay 
 * within the game boundaries along the x and z axis.
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
    [HideInInspector]
    public bool canTeleport;
    [HideInInspector]
    public bool canFireDualShot;
    [HideInInspector]
    public bool canAbsorbDamage;
    [HideInInspector]
    public float currentHealth = 1;
    [HideInInspector]
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
    private AudioClip takeDamageClip;
    private AudioClip slowDownClip;
    private float horizontalMovementSmoothing = 0.40f;
    private float verticalMovementSmoothing = 0.20f;

    public void PickupTeleport()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        _audio.clip = teleportPickupClip;
        _audio.Play();
        canTeleport = true;
        GameController.instance.UpdateTeleportStatusIcon("ACTIVE");
    }
    public void PickupArmor()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        _audio.clip = armorPickupClip;
        _audio.Play();
        canAbsorbDamage = true;
        GameController.instance.UpdateArmorStatusIcon("ACTIVE");
    }

    public void PickupDualShot()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.VOICES_VOLUME);
        _audio.clip = dualShotPickup;
        _audio.Play();
        canFireDualShot = true;
        GameController.instance.UpdateDualShotStatusIcon("ACTIVE");
    }

    public void PickupHealth(float amt)
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        _audio.clip = healthPickupClip;
        _audio.Play();

        if (currentHealth == maxHealth)
            return;
        else
        {
            currentHealth += amt;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
        GameController.instance.RefreshHealthBar(amt, false);
    }
    public void DecreaseSpeed()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        _audio.clip = slowDownClip;
        _audio.Play();
        currentSpeed = currentSpeed / 2;
    }
    public void NormalizeSpeed()
    {
        currentSpeed = maxSpeed;
    }

    public void AbsorbDamage()
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        _audio.clip = armorUsedClip;
        _audio.Play();
        StartCoroutine(GameController.instance.ArmorActive());
        canAbsorbDamage = false;
        GameController.instance.UpdateArmorStatusIcon("DEACTIVE");
    }

    public void TakeDamage(float damageAmt)
    {
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        _audio.clip = takeDamageClip;
        _audio.Play();

        if (currentHealth > 0.5f)
        {
            currentHealth -= damageAmt;
            if (currentHealth <= 0)
                GameController.instance.isPlayerDead = true;
            else
                StartCoroutine(GameController.instance.PlayerShipHit());
        }
        else
        {
            currentHealth = 0;
            GameController.instance.isPlayerDead = true;
        }

        GameController.instance.RefreshHealthBar(damageAmt, true);
    }

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
        takeDamageClip = Resources.Load<AudioClip>("Audio/takedamage");
        slowDownClip = Resources.Load<AudioClip>("Audio/slowdown");
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
           rb.position = new Vector3(
             Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, gameBoundary.xMin, gameBoundary.xMax),
             0.0f, 
             Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).z, gameBoundary.zMin, gameBoundary.zMax));
            _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
            _audio.clip = teleportUsedClip;
            _audio.Play();
            canTeleport = false;
            GameController.instance.UpdateTeleportStatusIcon("DEACTIVE");
        }
        else if (CrossPlatformInputManager.GetButtonDown("Jump") && canFire && !GameController.instance.isGamePaused)
        {
            canFire = false;
            _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
            if (canFireDualShot)
            {
                Instantiate(bolt, dualShotSpawn[0].position, Quaternion.Euler(0, 0, 0));
                Instantiate(bolt, dualShotSpawn[1].position, Quaternion.Euler(0, 0, 0));
                _audio.clip = dualShotClip;
                _audio.Play();
                canFireDualShot = false;
                GameController.instance.UpdateDualShotStatusIcon("Deactive");
            }
            else
            {
                Instantiate(bolt, singleShotSpawn.position, Quaternion.Euler (0, 0, 0));
                _audio.clip = singleShotClip;
                _audio.Play();
            }
        }
        else if (CrossPlatformInputManager.GetButtonUp("Jump"))
        {
            canFire = true;
        }
    }

    // FixedUpdate is called once, zero, or several times per frame
    private void FixedUpdate()
    {
        float movementHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float movementVertical = CrossPlatformInputManager.GetAxis("Vertical");
        Vector3 movement = new Vector3(movementHorizontal*horizontalMovementSmoothing, 0.0f, movementVertical*verticalMovementSmoothing);
        rb.velocity = movement * currentSpeed;
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, gameBoundary.xMin, gameBoundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, gameBoundary.zMin, gameBoundary.zMax));

        rb.rotation = Quaternion.Euler(90.0f, 0.0f, rb.velocity.x * -tiltX);
    }

    private bool hasTouchedValidLocation()
    {
        return Input.GetButtonDown("Fire1") && Camera.main.ScreenToWorldPoint(Input.mousePosition).z >= gameBoundary.zMin;
    }
}