using System.Collections;
using UnityEngine;

/**
 * Description: Regulates an enemy's actions, both movement and combat, including
 * moving forward, evading, and firing.
 * 
 * Details -
 * Awake: Instantiate and initialize audio source
 * Start: Intialize access to rigidbody component and move forward while repeatedly firing and evading
 * Fire: Instantiate shot and play audio.
 * MoveForward: Set velocity of rigidbody to move on the Z axis at a set speed
 * NextDirection: Calculate left or right direction using sign function on X position to give 1 or -1
 * Evade: Calculate target move, either left or right along x axis, wait, set target move to 0, wait, and repeat
 * FixedUpdate: Calcuate new velocity using target move and move forward with an added tilt to mimic flying through space
 */
public class EnemyController : MonoBehaviour
{
    public float speedZ;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float fireDelay;

    public float dodgeX;
    public float smoothingX;
    public float tiltX;
    public Vector2 startWait;
    public Vector2 moveTime;
    public GameBoundary gameBoundary;

    private float targetMoveX;
    private Rigidbody rb;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MoveForward();
        InvokeRepeating("Fire", fireDelay, fireRate);
        StartCoroutine(Evade());
    }

    void Fire()
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        _audio.Play();
    }

    void MoveForward()
    {
        rb.velocity = transform.forward * speedZ;
    }

    float NextDirection()
    {
        return -Mathf.Sign(transform.position.x);
    }
    
    IEnumerator Evade()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));

        while (true)
        {
            targetMoveX = Random.Range(1, dodgeX) * NextDirection();
            yield return new WaitForSeconds(Random.Range(moveTime.x, moveTime.y));
            targetMoveX = 0;
            yield return new WaitForSeconds(Random.Range(moveTime.x, moveTime.y));
        }
    }

    // FixedUpdate is called once, zero, or several times per frame
    void FixedUpdate()
    {
        float nextMoveX = Mathf.MoveTowards(rb.velocity.x, targetMoveX, Time.deltaTime * smoothingX);
        rb.velocity = new Vector3(nextMoveX, 0.0f, speedZ);
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, gameBoundary.xMin, gameBoundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, gameBoundary.zMin, gameBoundary.zMax));

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tiltX);
    }
}