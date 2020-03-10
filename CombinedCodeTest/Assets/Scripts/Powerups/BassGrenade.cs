using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BassGrenade : MonoBehaviour
{
    public float GrowthSpeed = 3;
    public int MaxSize = 30;
    //private VZPlayer score; //Moved to CoinCollector.cs
    public Material Effect;
    public Material Skybox;

    // Start is called before the first frame update
    void Start()
    {
        //score = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>(); //Moved to CoinCollector.cs
        RenderSettings.skybox = Effect;

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale += new Vector3(1,1,1) * GrowthSpeed * Time.deltaTime;
        if (transform.localScale.x >= MaxSize) //Since everything scales uniformly, this works fine.
        {
            CheckForMultiples(); //This affects whether the skybox changes back or not.
            Destroy(this.gameObject);
        }
    }

    //If there are multiple bass grenades active by the time the effect is supposed to disappear, it doesn't change the skybox back (so that the last one to disappear can do that instead).
    void CheckForMultiples()
    {
        GameObject[] BassGrenades = GameObject.FindGameObjectsWithTag("BassGrenade");
        //Debug.Log(BassGrenades.Length);
        if (BassGrenades.Length <= 1)
        {
            RenderSettings.skybox = Skybox;
        }
        else
        {
            //Debug.Log("There's multiple Bass Grenades going off! Not gonna change the skybox back!");
        }
    }

    //Moved to "CoinCollector.cs"
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("2Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 2;
        }
        if (other.gameObject.CompareTag("4Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 4;
        }
        if (other.gameObject.CompareTag("8Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 8;
        }
        if (other.gameObject.CompareTag("16Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 16;
        }
        if (other.gameObject.CompareTag("32Bit"))
        {
            Destroy(other.gameObject);
            score.Score += 32;
        }
    }*/
}
