using UnityEngine;
/**
 * Description: Add random rotation to the associated game object
 * 
 * Details: Using a random Vector3 value from Random.insideUnitSphere function 
 * we change the angular velocity of the game object’s rigidbody. The random vector3 
 * is additionally modified by a roll value to increase the speed of the rotation from 
 * the default 1 provided by insideUnitSphere.
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