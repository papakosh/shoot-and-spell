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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
            return;
        if (gameObject.CompareTag("Health") && other.CompareTag("Bolt"))
            return;
        if (gameObject.CompareTag("Shield") && other.CompareTag("Bolt"))
            return;

        if (gameObject.CompareTag("Health"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                PlayerController.instance.IncreaseHealth(damage);

            return;
        }
        // compare tag "Double Bolt", set double bolt ability to true
        if (gameObject.CompareTag("Double Bolt"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                PlayerController.instance.doubleBoltAbility = true;

            return;
        }
        if (gameObject.CompareTag("Shield"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                PlayerController.instance.bufferAbility = true;

            return;
        }

        if (explosion != null)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance.bufferAbility)
            {
                PlayerController.instance.ShieldsUp();
            }
            else
            {

                PlayerController.instance.DecreaseHealth(damage);
                if (PlayerController.instance.isDead)
                {
                    Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                    GameController.instance.GameLose();
                }
            }
        }

        if (gameObject.CompareTag("Hazard"))
        {
            int num = UnityEngine.Random.Range(1, 6);
            Transform pickupTransform = transform;
            Quaternion rotateQuaternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            // game controller spawn random pickup - health or double bolt or invinicibily or wormhole
            GameController.instance.SpawnRandomPickup(pickups, pickupTransform, rotateQuaternion);
        }
        else
        {
            // instantiate gameobject to play sound
            Boolean goodHit = GameController.instance.ProcessHit(gameObject.tag);
            /*if (GameController.instance.inAlphabetMode())
            {
                if (goodHit) { 
                 if (success != null)
                    Instantiate(success);
                }
                else
                {
                    if (failure != null)
                        Instantiate(failure);
                }

            }*/
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}