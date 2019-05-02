using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    public GameObject playerExplosion;
    public GameObject explosion;
    public float damage;
    public GameObject pickup;
    public GameObject success;
    public GameObject failure;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
            return;
        if (gameObject.CompareTag("Health") && other.CompareTag("Bolt"))
            return;

        if (gameObject.CompareTag("Health"))
        {
           // Debug.Log("Contact");
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
                Destroy(other.gameObject);
            else
                PlayerController.instance.IncreaseHealth(damage);

            return;
        }
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (other.CompareTag("Player"))
        {
            PlayerController.instance.DecreaseHealth(damage);
            if (PlayerController.instance.isDead)
            {
                Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                Destroy(other.gameObject);
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

            switch (num)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    Instantiate(pickup, pickupTransform.position, rotateQuaternion);
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    break;
            }

            //PlayerController.instance.HealthPickup();
        }
        else
        {
            // instantiate gameobject to play sound
            Boolean goodHit = GameController.instance.ProcessHit(gameObject.tag);
            if (GameController.instance.inAlphabetMode())
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

            }
            Destroy(gameObject);
            if (!other.CompareTag("Player"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}