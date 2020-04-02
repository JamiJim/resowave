using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousEmail : MonoBehaviour
{
    public GameObject[] Effects;
    //public AudioSource aSource;
    //public AudioClip aSound;


    private void PowerupEffect()
    {
        Instantiate(Effects[Random.Range(0, (Effects.Length - 1))], this.transform.position, Quaternion.identity); //A random powerup effect is used, from a list specified in the editor.
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
