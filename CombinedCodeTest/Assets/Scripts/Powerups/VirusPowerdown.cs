using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusPowerdown : MonoBehaviour
{
    private VZPlayer score;
    public int ScoreDecrement = 128;

    private void Start()
    {
        score = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (score.Score < ScoreDecrement)
            {
                score.Score = 0;
            }
            else
            {
                score.Score -= 128;
            }
            Destroy(this.gameObject);
        }
    }
}
