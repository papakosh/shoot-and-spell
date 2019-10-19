using UnityEngine;

/**
 * Description: Destroy the associated game object after a specified time
 * 
 * Details: Start calls Destroy on the asscoiated game object and
 * destroys it after a specified time.
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
