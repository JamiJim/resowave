using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    //This script is used for any Powerup that collects coins while it's active (Bass Grenade, Net, etc.)
    private VZPlayer source;

    private void Start()
    {
        source = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("2Bit"))
        {
            Destroy(other.gameObject);
            source.Score += 2 * source.ScoreMultiplier;
            source.aSource.PlayOneShot(source.aSound[0], (MenuControl.SoundEffectVolume * 0.01f));
        }
        if (other.gameObject.CompareTag("4Bit"))
        {
            Destroy(other.gameObject);
            source.Score += 4 * source.ScoreMultiplier;
            source.aSource.PlayOneShot(source.aSound[1], (MenuControl.SoundEffectVolume * 0.01f));
        }
        if (other.gameObject.CompareTag("8Bit"))
        {
            Destroy(other.gameObject);
            source.Score += 8 * source.ScoreMultiplier;
            source.aSource.PlayOneShot(source.aSound[2], (MenuControl.SoundEffectVolume * 0.01f));
        }
        if (other.gameObject.CompareTag("16Bit"))
        {
            Destroy(other.gameObject);
            source.Score += 16 * source.ScoreMultiplier;
            source.aSource.PlayOneShot(source.aSound[3], (MenuControl.SoundEffectVolume * 0.01f));
        }
        if (other.gameObject.CompareTag("32Bit"))
        {
            Destroy(other.gameObject);
            source.Score += 32 * source.ScoreMultiplier;
            source.aSource.PlayOneShot(source.aSound[4], (MenuControl.SoundEffectVolume * 0.01f));
        }
    }
}
