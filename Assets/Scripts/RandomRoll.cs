using UnityEngine;
/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Add random rotation to the associated game object
 * 
 * Details: 
 * Attributes-
 * roll - speed of rotation
 * 
 * Methods-
 * Start - Set the angular velocity of an associated game object to 1 (from
 * Random.insideUnitSphere) multiplied by a roll value.
 */
public class RandomRoll : MonoBehaviour
{
    public float roll;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Random.insideUnitSphere * roll;
    }

}