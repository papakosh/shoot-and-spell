using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

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
    public Transform shotSpawnDual1;
    public Transform shotSpawnDual2;
    public GameObject shot;
    public Boundary boundary;
    public float tiltModifier;
    public static PlayerController instance = null;
    private AudioSource _audio;
    [HideInInspector]
    public float originalSpeed;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();
        _audio = audioSources[0];
        originalSpeed = speed;

    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canFire = true;
        CalibrateAccelerometer();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.teleportAbility && !GameController.instance.isPaused && Input.GetButtonDown("Fire1") && Camera.main.ScreenToWorldPoint(Input.mousePosition).z >=0)
        {
           rb.position = new Vector3(Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, boundary.xMin, boundary.xMax),
             0.0f, Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).z, boundary.zMin, boundary.zMax));

            GameController.instance.TeleportActivated();
            GameController.instance.teleportAbility = false;
            return;
        }
        else if (CrossPlatformInputManager.GetButtonDown("Jump") && canFire && !GameController.instance.isPaused)
        {
            if (GameController.instance.dualShotAbility)
            {
                Instantiate(shot, shotSpawnDual1.position, shotSpawnDual1.rotation);
                Instantiate(shot, shotSpawnDual2.position, shotSpawnDual2.rotation);

                AudioSource[] audioSources = GetComponents<AudioSource>();
                _audio = audioSources[1];
                _audio.Play();
                _audio = audioSources[0];

                GameController.instance.ResetDualShot ();
            }
            else
            {
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
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
    
}