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
                Instantiate(explosion, transform.position, transform.rotation);
            } else if (other.CompareTag("Player")){
                GameController.instance.ProcessHit(gameObject.tag);
                Destroy(gameObject);
                Instantiate(explosion, transform.position, transform.rotation);
                if (GameController.instance.armorAbility)
                {
                    GameController.instance.ArmorActivated();
                }
                else
                {
                    GameController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                    }
                }
            }
        }else if (gameObject.CompareTag("Health"))  // Is a Health pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard"))
                return;
            else if (other.CompareTag("Player"))
            {
                GameController.instance.IncreaseHealth(damage);
                Destroy(gameObject);
                return;
            }
        }else if (gameObject.CompareTag("Double Bolt")) // Is a Dual Shot pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard"))
                return;
            else if (other.CompareTag("Player")) {
                GameController.instance.DualShotPickup();
                Destroy(gameObject);
                return;
            }
         }else if (gameObject.CompareTag("Shield")) // Is a Shield pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard") || ALetter(other.gameObject.tag))
                return;
            else if (other.CompareTag("Player"))
            {
                GameController.instance.ArmorPickup();
                Destroy(gameObject);
                return;
            }
        }else if (gameObject.CompareTag("Teleport")) // Is a Teleport pickup?
        {
            if (other.CompareTag("Bolt") || other.CompareTag("Enemy") || other.CompareTag("Hazard") || ALetter(other.gameObject.tag))
                return;
            else if (other.CompareTag("Player"))
            {
                GameController.instance.TeleportPickup();
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
            Instantiate(explosion, transform.position, transform.rotation);

            if (other.CompareTag("Bolt"))
            {
                Destroy(other.gameObject);
                GameController.instance.SpawnRandomPickup(pickups, pickupTransform, rotateQuaternion);
                
            }
            else if (other.CompareTag("Player"))
            {
                if (GameController.instance.armorAbility)
                {
                    GameController.instance.ArmorActivated();
                }
                else
                {
                    GameController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
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
                    Instantiate(explosion, transform.position, transform.rotation);
            }
            else if (other.CompareTag("Player"))
            {
                Destroy(gameObject);
                if (explosion != null)
                    Instantiate(explosion, transform.position, transform.rotation);
                if (GameController.instance.armorAbility)
                {
                    GameController.instance.ArmorActivated();
                }
                else
                {
                    GameController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                    }
                }
            }
            return;
        }

        /*if (gameObject.CompareTag("Health") && (other.CompareTag("Bolt") || other.CompareTag("Enemy"))) // Ignore collisions between health pickup and either bolt or enemy
            return;
        if (gameObject.CompareTag("Shield") && (other.CompareTag("Bolt") || other.CompareTag("Enemy"))) // Ignore collisions between shield pickup and either bolt or enemy
            return;
        if (gameObject.CompareTag("Double Bolt") && (other.CompareTag("Bolt") || other.CompareTag("Enemy"))) // Ignore collisions between double bolt pickup and either bolt or enemy
            return;
        if (gameObject.CompareTag("Teleport") && (other.CompareTag("Bolt") || other.CompareTag("Enemy"))) // Ignore collisions between teleport pickup and either bolt or enemy
            return;

        if (gameObject.CompareTag("Enemy") || other.CompareTag("Enemy"))
        {
            if (!other.CompareTag("Player") && !other.CompareTag ("Bolt"))
                return;
        }

        if (gameObject.CompareTag("Health"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                GameController.instance.IncreaseHealth(damage);

            return;
        }
        if (gameObject.CompareTag("Double Bolt"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                GameController.instance.DualShotPickup();

            return;
        }
        if (gameObject.CompareTag("Shield"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                GameController.instance.ArmorPickup();

            return;
        }

        if (gameObject.CompareTag("Teleport"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                GameController.instance.TeleportPickup();
            return;
        }*/

            /* if (explosion != null)
             {
                 Instantiate(explosion, transform.position, transform.rotation);
             }*/

            /*if (other.CompareTag("Player"))
            {
                if (GameController.instance.armorAbility)
                {
                    GameController.instance.ArmorActivated();
                }
                else
                {

                    GameController.instance.DecreaseHealth(damage);
                    if (GameController.instance.isDead)
                    {
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                    }
                }
            }
            */
            /*if (gameObject.CompareTag("Hazard"))
            {
                int num = UnityEngine.Random.Range(1, 6);
                Transform pickupTransform = transform;
                Quaternion rotateQuaternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

                Destroy(gameObject);
                if (!other.CompareTag("Player"))
                    Destroy(other.gameObject);
                GameController.instance.SpawnRandomPickup(pickups, pickupTransform, rotateQuaternion);
            }
            else
            {
                    GameController.instance.ProcessHit(gameObject.tag);
                    Destroy(gameObject);
                    if (!other.CompareTag("Player"))
                    {
                        Destroy(other.gameObject);
                    }

            }*/
    }

    private bool ALetter(String tag)
    {
        Char[] vals = tag.ToCharArray();
        return vals.Length == 1 && ((int)vals[0] >= 65 && (int)vals[0] <= 90);
    }
}