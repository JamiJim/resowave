using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupGeneric : MonoBehaviour
{
    public GameObject Effect;
    public bool MultiplesAllowed = true; //If multiples of the powerup can be onscreen at once, set this to true.
    public string SingularTag; //Specify the tag name of the Powerup that is to be limited to 1 on-screen. Ignore this if MultiplesAllowed is set to true.
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
            //Currently, every Powerup can work in multiples, and was from an earlier version. It remains here in case such a mechanic is needed again.
            if (MultiplesAllowed == false) //Can more than one of this powerup effect be onscreen at once?
            {
                if (GameObject.FindGameObjectWithTag(SingularTag)) //If not, check to see if there IS already one onscreen.
                {
                    Destroy(this.gameObject); //If there is, destroy this powerup without spawning its effect.
                }
                else
                {
                    PowerupEffect(); //If this powerup's effect ISN'T already on-screen, spawn the effect, as normal.
                }
            }
            else
            {
                PowerupEffect(); //If multiples of this powerup's effect can be on-screen at once, just spawn the effect.
            }
        }
    }
}
