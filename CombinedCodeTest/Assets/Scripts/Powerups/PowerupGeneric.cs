using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupGeneric : MonoBehaviour
{
    public GameObject Effect;
    //public AudioSource aSource;
    //public AudioClip aSound;

    private void PowerupEffect() //What normally happens when the powerup is collected. Spawns the effect, then disappears.
    {
        Instantiate(Effect.gameObject, this.transform.position, Quaternion.identity);
        //aSource.PlayOneShot(aSound, 1.0f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PowerupEffect();
        }
    }
}
