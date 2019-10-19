using UnityEngine;

/**
 * Description: Destroy any game object leaving the boundaries of 
 * the game.
 * 
 * Details: OnTriggerExit calls Destroy on the exiting game object
 */
public class DestroyByBoundary : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
           Destroy(other.gameObject);
    }
}
