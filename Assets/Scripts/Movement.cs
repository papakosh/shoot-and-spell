using UnityEngine;
/**
 * Description: Sets the direction and speed of the associated game object
 * 
 * Details: Multiplying the Vector3 of Transform.forward by a speed value, we change 
 * the velocity of the game object’s rigidbody along the Z-axis.
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