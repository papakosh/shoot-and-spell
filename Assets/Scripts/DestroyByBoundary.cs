using UnityEngine;

/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Destroy any game object leaving the boundaries of 
 * the game.
 * 
 * Details: 
 * Methods-
 * OnTriggerExit: Calls Destroy on the colliding game object
 */
public class DestroyByBoundary : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
           Destroy(other.gameObject);
    }
}
