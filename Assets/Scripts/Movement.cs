using UnityEngine;
/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Sets the direction and speed of the associated game object
 * 
 * Details:
 * Attributes-
 * speedZ - Speed on the Z axis when moving down toward the player
 * 
 * Mathods-
 * Start: Move the associated game object (like an asteroid) forward
 * at specified speed by multiplying rigidbody's transform.forward
 * by speedZ
 */
public class Movement : MonoBehaviour
{
    public float speedZ;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speedZ;
    }

}