using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusPowerdown : MonoBehaviour
{
    private VZPlayer Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Player.Score < Player.VirusDecrement)
            {
                Player.Score = 0;
            }
            else
            {
                Player.Score -= Player.VirusDecrement;
            }
            Destroy(this.gameObject);
        }
    }
}
