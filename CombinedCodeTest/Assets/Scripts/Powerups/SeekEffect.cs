using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekEffect : MonoBehaviour
{
    private GameObject Player;
    public float MagnificationSpeed;
    public float Lifetime;
    private static float lifetime;
    private GameObject[] Seeks;

    //Affects the Geometry
    public GameObject Geometry;
    public float ScaleSpeed = 8f;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("BikeInputCont");
        Seeks = GameObject.FindGameObjectsWithTag("Seek"); //This is done as a check for multiples.

        if (Seeks.Length > 1) //If a /Seek has been collected at the same time one is already active...
        {
            lifetime += Lifetime; //The time is increased by the Lifetime of the object...
            Destroy(Seeks[1].gameObject); //And the new one (this one) is removed.
        }
        else
        {
            lifetime = Lifetime; //However, if there ARE no other /Seeks active, everything continues as normal.
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Player.transform.position; //The net and its children track the player's position.

        lifetime -= Time.deltaTime; //The object's life ticks down.

        if (lifetime <= 0)
        {
            Geometry.transform.localScale -= (new Vector3(1, 1, 1) * Time.deltaTime * ScaleSpeed); //The visual effect disappears first.
            if (Geometry.gameObject.transform.localScale.x <= 0)
            {
                Destroy(this.gameObject); //Once the geometry is no longer visible, the object is invisible.
            }
        }
    }

    //The magnification.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("2Bit") || other.CompareTag("4Bit") || other.CompareTag("8Bit") || other.CompareTag("16Bit") || other.CompareTag("32Bit"))
        {
            other.gameObject.transform.position = Vector3.MoveTowards(other.transform.position, Player.transform.position, MagnificationSpeed * Time.deltaTime);
        }
    }
}
