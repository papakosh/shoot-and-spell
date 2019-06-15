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
        if (gameObject.CompareTag("Double Bolt") && other.CompareTag("Bolt"))
            return;

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
                GameController.instance.doubleBoltAbility = true;

            return;
        }
        if (gameObject.CompareTag("Shield"))
        {
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                GameController.instance.bufferAbility = true;

            return;
        }

        if (explosion != null)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (other.CompareTag("Player"))
        {
            if (GameController.instance.bufferAbility)
            {
                GameController.instance.ShieldsUp();
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

        if (gameObject.CompareTag("Hazard"))
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
            Boolean goodHit = GameController.instance.ProcessHit(gameObject.tag);
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}