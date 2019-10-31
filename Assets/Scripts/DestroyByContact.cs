using System;
using UnityEngine;

/**
 * Description: Process collisions between the attached game object, including letters, pickups 
 * (health, dual shot, armor, teleport), hazards, and enemies, and a collider, including any
 * of the previous plus the game boundary, bolts, and the player.
 * 
 * Details:
 * OnTriggerEnter:
 * CollisionIsWithALetter: Return true if tag's ascii value is between 65 and 90 (A to Z)
 * CollisionIsWithTheBoundary: Return true if tag is equal to Game Boundary
 * CollisionIsWithABolt: Return true if tag is equal to Bolt
 * CollisionIsWithThePlayer: Return true if tag is equal to Player
 * CollisionIsWithAHealthPickup: Return true if tag is equal to Health
 * CollisionIsWithADualShotPickup: Return true if tag is equal to Dual Shot
 * CollisionIsWithAnArmorPickup: Return true if tag is equal to Armor
 * CollisionIsWithATeleportPickup: Return true if tag is equal to Teleport
 * CollisionIsWithAnyPickup: Return true if tag is equal to Health,  Dual Shot, Armor, or
 * Teleport
 * CollisionIsWithAnEnemy: Return true if tag is Enemy
 * CollisionIsWithAnEnemyBolt: Return true if tag is Enemy Bolt
 * CollisionIsWithAHazard: Return true if tag is Hazard
 */
public class DestroyByContact : MonoBehaviour
{
    public GameObject playerExplosion;
    public GameObject explosion;
    public float damage;

    void OnTriggerEnter(Collider other)
    {
        if (CollisionIsWithTheGameBoundary(other.gameObject.tag))
            return;
        
        if (CollisionIsWithALetter(gameObject.tag))
        {
            if (CollisionIsWithAnEnemy(other.gameObject.tag) || CollisionIsWithAHazard(other.gameObject.tag) ||
                CollisionIsWithALetter(other.gameObject.tag) || CollisionIsWithAnyPickup(other) ||
                CollisionIsWithAnEnemyBolt(other.gameObject.tag))
                return;
            string letter = gameObject.tag;
            Destroy(gameObject);
            explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
            Instantiate(explosion, transform.position, transform.rotation);
            if (CollisionIsWithABolt(other.gameObject.tag)) Destroy(other.gameObject);
            else if (CollisionIsWithThePlayer(other.gameObject.tag)){
                if (PlayerController.instance.canAbsorbDamage) PlayerController.instance.AbsorbDamage();
                else
                {
                    PlayerController.instance.TakeDamage(damage);
                    if (GameController.instance.isPlayerDead)
                    {
                        playerExplosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
                        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        GameController.instance.RoundLose();
                        return;
                    }
                }   
            }
            GameController.instance.ProcessHit(letter);
        }
        else if (CollisionIsWithAHealthPickup(gameObject.tag) || CollisionIsWithADualShotPickup(gameObject.tag) || 
            CollisionIsWithAnArmorPickup(gameObject.tag) || CollisionIsWithATeleportPickup(gameObject.tag)){
            if (CollisionIsWithABolt(other.gameObject.tag) || CollisionIsWithAnEnemy(other.gameObject.tag) || 
                CollisionIsWithAHazard(other.gameObject.tag) || CollisionIsWithALetter(other.gameObject.tag) ||
                CollisionIsWithAnyPickup(other) || CollisionIsWithAnEnemyBolt(other.gameObject.tag))
                return;
            else if (CollisionIsWithThePlayer(other.gameObject.tag))
            {
                if (CollisionIsWithAHealthPickup(gameObject.tag)) PlayerController.instance.PickupHealth(damage);
                else if (CollisionIsWithADualShotPickup(gameObject.tag)) PlayerController.instance.PickupDualShot();
                else if (CollisionIsWithAnArmorPickup(gameObject.tag)) PlayerController.instance.PickupArmor();
                else if (CollisionIsWithATeleportPickup(gameObject.tag)) PlayerController.instance.PickupTeleport();

                Destroy(gameObject);
                return;
            }
        }else if (CollisionIsWithAHazard(gameObject.tag))
        {
            if (CollisionIsWithAnEnemy(other.gameObject.tag) || CollisionIsWithAHazard(other.gameObject.tag) || 
                CollisionIsWithALetter(other.gameObject.tag) || CollisionIsWithAnyPickup(other) ||
                CollisionIsWithAnEnemyBolt(other.gameObject.tag))
                return;
            Transform pickupTransform = transform;
            Destroy(gameObject);
            explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
            Instantiate(explosion, transform.position, transform.rotation);

            if (CollisionIsWithABolt(other.gameObject.tag))
            {
                Destroy(other.gameObject);
                GameController.instance.SpawnRandomPickup(pickupTransform);
            }
            else if (CollisionIsWithThePlayer(other.gameObject.tag))
            {
                if (PlayerController.instance.canAbsorbDamage) PlayerController.instance.AbsorbDamage();
                else
                {
                    PlayerController.instance.TakeDamage(damage);
                    if (GameController.instance.isPlayerDead)
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
        else if (CollisionIsWithAnEnemy(gameObject.tag))
        {
            if (CollisionIsWithAnEnemy(other.gameObject.tag) || CollisionIsWithAHazard(other.gameObject.tag) || 
                CollisionIsWithALetter(other.gameObject.tag) || CollisionIsWithAnyPickup(other) ||
                CollisionIsWithAnEnemyBolt(other.gameObject.tag))
                return;
            Transform pickupTransform = transform;
            Destroy(gameObject);
            explosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataController.EXPLOSIONS_VOLUME);
            Instantiate(explosion, transform.position, transform.rotation);
        
            if (CollisionIsWithABolt(other.gameObject.tag))
            {
                Destroy(other.gameObject);
                GameController.instance.SpawnRandomPickup(pickupTransform);
            }
            else if (CollisionIsWithThePlayer(other.gameObject.tag))
            {
                if (PlayerController.instance.canAbsorbDamage) PlayerController.instance.AbsorbDamage();
                else
                {
                    PlayerController.instance.TakeDamage(damage);
                    if (GameController.instance.isPlayerDead)
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
        else if (CollisionIsWithAnEnemyBolt(gameObject.tag))
        {
            if (CollisionIsWithAnEnemy(other.gameObject.tag) || CollisionIsWithAHazard(other.gameObject.tag) ||
                CollisionIsWithALetter(other.gameObject.tag) || CollisionIsWithAnyPickup(other) ||
                CollisionIsWithAnEnemyBolt(other.gameObject.tag))
                return;
            Destroy(gameObject);

            if (CollisionIsWithABolt(other.gameObject.tag)) Destroy(other.gameObject);
            else if (CollisionIsWithThePlayer(other.gameObject.tag))
            {
                if (PlayerController.instance.canAbsorbDamage) PlayerController.instance.AbsorbDamage();
                else
                {
                    PlayerController.instance.TakeDamage(damage);
                    if (GameController.instance.isPlayerDead)
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

    private bool CollisionIsWithALetter(string tag)
    {
        Char[] vals = tag.ToCharArray();
        return vals.Length == 1 && ((int)vals[0] >= 65 && (int)vals[0] <= 90);
    }

    private bool CollisionIsWithTheGameBoundary (string tag)
    {
        return tag.Equals("Game Boundary");
    }

    private bool CollisionIsWithABolt(string tag)
    {
        return tag.Equals("Bolt");
    }
    private bool CollisionIsWithThePlayer(string tag)
    {
        return tag.Equals("Player");
    }

    private bool CollisionIsWithAHealthPickup(string tag)
    {
        return tag.Equals("Health");
    }
    private bool CollisionIsWithADualShotPickup(string tag)
    {
        return tag.Equals("Dual Shot");
    }
    private bool CollisionIsWithAnArmorPickup(string tag)
    {
        return tag.Equals("Armor");
    }
    private bool CollisionIsWithATeleportPickup(string tag)
    {
        return tag.Equals("Teleport");
    }
    private bool CollisionIsWithAnyPickup(Collider other)
    {
        return CollisionIsWithAHealthPickup(other.gameObject.tag) || CollisionIsWithADualShotPickup(other.gameObject.tag) ||
            CollisionIsWithAnArmorPickup(other.gameObject.tag) || CollisionIsWithATeleportPickup(other.gameObject.tag);
    }
    private bool CollisionIsWithAnEnemy(string tag)
    {
        return tag.Equals("Enemy");
    }
    private bool CollisionIsWithAnEnemyBolt(string tag)
    {
        return tag.Equals("Enemy Bolt");
    }
    private bool CollisionIsWithAHazard(string tag)
    {
        return tag.Equals("Hazard");
    }
}