using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    //This script is used for any Powerup that collects coins while it's active (Bass Grenade, Net, etc.)
    private VZPlayer score;

    private void Start()
    {
        score = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("2Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 2 * score.ScoreMultiplier;
        }
        if (other.gameObject.CompareTag("4Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 4 * score.ScoreMultiplier;
        }
        if (other.gameObject.CompareTag("8Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 8 * score.ScoreMultiplier;
        }
        if (other.gameObject.CompareTag("16Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 16 * score.ScoreMultiplier;
        }
        if (other.gameObject.CompareTag("32Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 32 * score.ScoreMultiplier;
        }
    }
}
