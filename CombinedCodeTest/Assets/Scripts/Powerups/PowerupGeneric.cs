using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupGeneric : MonoBehaviour
{
    public GameObject Effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(Effect.gameObject, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
