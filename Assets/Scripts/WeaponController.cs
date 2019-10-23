using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float delay;

    private AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        _audio.volume = PlayerPrefs.GetFloat(DataController.WEAPONS_VOLUME);
        InvokeRepeating("Fire", delay, fireRate);
    }

    void Fire()
    {
       Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        _audio.Play();
    }
}