using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Health"))
          //  Debug.Log("Contact with Boundary");
           Destroy(other.gameObject);
    }
}
