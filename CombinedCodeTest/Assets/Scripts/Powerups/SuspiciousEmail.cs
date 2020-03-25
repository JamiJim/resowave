using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousEmail : MonoBehaviour
{
    public GameObject[] Effects;
    private VZPlayer Player;
    private int RandomRange;
    //public AudioSource aSource;
    //public AudioClip aSound;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
    }
    private void PowerupEffect()
    {
        RandomRange = Random.Range(0, (Effects.Length + 1));
        if (RandomRange >= Effects.Length) //There's a chance that the Suspicious Email will act like a virus, rather than like a Power-Up.
        {
            if (Player.Score < Player.VirusDecrement)
            {
                Player.Score = 0;
            }
            else
            {
                Player.Score -= Player.VirusDecrement;
            }
            Debug.Log("Virus!");
        }
        else {
            Instantiate(Effects[RandomRange], this.transform.position, Quaternion.identity); //A random powerup effect is used, from a list specified in the editor.
            Debug.Log("Power-Up!");
        }
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
