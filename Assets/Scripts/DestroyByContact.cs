using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    public GameObject playerExplosion;
    public GameObject explosion;
    public float damage;
    public GameObject[] pickups;
    public GameObject success;
    public GameObject failure;

    // Game object can be health pickup, shield pickup, double bolt pickup, teleport pickup, asteroids, letters and enemy
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Boundary")) // Ignore any collisions with boundary
            return;

        
        if (ALetter(gameObject.tag))   // Is A Letter?
        {
            if (other.CompareTag("Bolt"))
            {
                GameController.instance.ProcessHit(gameObject.tag);
                Destroy(gameObject);
                Destroy(other.gameObject);
                explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                Instantiate(explosion, transform.position, transform.rotation);
            } else if (other.CompareTag("Player")){
                Destroy(gameObject);
                explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                Instantiate(explosion, transform.position, transform.rotation);
                if (PlayerController.instance.canAbsorbDamage) //GameController.instance.armorAbility
                {
                    //GameController.instance.ArmorActivated();
                    PlayerController.instance.AbsorbDamage();
                    GameController.instance.ProcessHit(gameObject.tag);
                }
                else
                {
                    //GameController.instance.DecreaseHealth(damage);
                    PlayerController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
                        playerExplosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                        return;
                    }
                    GameController.instance.ProcessHit(gameObject.tag);
                }
            }
        }else if (gameObject.CompareTag("Health"))  // Is a Health pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard"))
                return;
            else if (other.CompareTag("Player"))
            {
                //GameController.instance.IncreaseHealth(damage);
                PlayerController.instance.IncreaseHealth(damage);
                Destroy(gameObject);
                return;
            }
        }else if (gameObject.CompareTag("Double Bolt")) // Is a Dual Shot pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard"))
                return;
            else if (other.CompareTag("Player")) {
                //GameController.instance.DualShotPickup();
                PlayerController.instance.PickupDualShot();
                Destroy(gameObject);
                return;
            }
         }else if (gameObject.CompareTag("Shield")) // Is a Shield pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard") || ALetter(other.gameObject.tag))
                return;
            else if (other.CompareTag("Player"))
            {
                //GameController.instance.ArmorPickup();
                PlayerController.instance.PickupArmor();
                Destroy(gameObject);
                return;
            }
        }else if (gameObject.CompareTag("Teleport")) // Is a Teleport pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard") || ALetter(other.gameObject.tag))
                return;
            else if (other.CompareTag("Player"))
            {
                //GameController.instance.TeleportPickup();
                PlayerController.instance.PickupTeleport();
                Destroy(gameObject);
                return;
            }
        }else if (gameObject.CompareTag("Hazard")) // Is a Hazard?
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Hazard") || ALetter(other.gameObject.tag))
                return;
            int num = UnityEngine.Random.Range(1, 6);
            Transform pickupTransform = transform;
            Quaternion rotateQuaternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

            Destroy(gameObject);
            explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
            Instantiate(explosion, transform.position, transform.rotation);

            if (other.CompareTag("Bolt"))
            {
                Destroy(other.gameObject);
                GameController.instance.SpawnRandomPickup(pickups, pickupTransform, rotateQuaternion);
                
            }
            else if (other.CompareTag("Player"))
            {
                if (PlayerController.instance.canAbsorbDamage) //GameController.instance.armorAbility
                {
                    //GameController.instance.ArmorActivated();
                    PlayerController.instance.AbsorbDamage();
                }
                else
                {
                    //GameController.instance.DecreaseHealth(damage);
                    PlayerController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
                        playerExplosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                    }
                }
            }
            return;
        }
        else if (gameObject.CompareTag("Enemy")) // Is an Enemy?
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Hazard") || ALetter(other.gameObject.tag))
                return;
            else if (other.CompareTag("Bolt"))
            {
                Destroy(gameObject);
                Destroy(other.gameObject);
                if (explosion != null)
                {
                    explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                    Instantiate(explosion, transform.position, transform.rotation);
                }
            }
            else if (other.CompareTag("Player"))
            {
                Destroy(gameObject);
                if (explosion != null)
                {
                    explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                    Instantiate(explosion, transform.position, transform.rotation);
                }
                if (PlayerController.instance.canAbsorbDamage) //GameController.instance.armorAbility
                {
                    //GameController.instance.ArmorActivated();
                    PlayerController.instance.AbsorbDamage();
                }
                else
                {
                    // GameController.instance.DecreaseHealth(damage);
                    PlayerController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
                        playerExplosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                    }
                }
            }
            return;
        }
    }

    private bool ALetter(String tag)
    {
        Char[] vals = tag.ToCharArray();
        return vals.Length == 1 && ((int)vals[0] >= 65 && (int)vals[0] <= 90);
    }
}