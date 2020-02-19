using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTerrain : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject, 0.1f);
        }

    }

}
