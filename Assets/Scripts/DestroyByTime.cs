using UnityEngine;

/**
 * Description: Destroy the associated game object after a specified time
 * 
 * Details: 
 * Attributes-
 * Lifetime - how many seconds before destroying an associated game object
 * 
 * Methods-
 * Start: Calls Destroy on the associated game object and destroys it after a specified time.
 */
public class DestroyByTime : MonoBehaviour
{
    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}