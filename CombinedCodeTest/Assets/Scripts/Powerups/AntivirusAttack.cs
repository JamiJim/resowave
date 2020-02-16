using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntivirusAttack : MonoBehaviour
{
    public float Speed;
    public float LifeTime = 4f; //How long the object exists before it starts to disappear.
    public float ScaleSpeed = 16f; //The speed at which the object disappears.
    private Vector3 target;
    private GameObject[] Virus;
    private int ListDeduction = 1;

    //Controls the Up-And-Down Movement of the Object. (SinY)
    public float HeightMinMax = 3; //How far up and down the object will move.
    public float modSpeed = 8; //How fast the object moves up and down.
    private float sinModifier = 0; //This what gives the object its slow movement, up and down.

    private void Update()
    {
        LifeTime -= Time.deltaTime; //The lifetime ticks down.

        //This is for the up-and-down movement.
        sinModifier += Time.deltaTime * modSpeed;
        this.transform.Translate(Vector3.up * Time.deltaTime * HeightMinMax * Mathf.Sin(sinModifier));

        //This is what makes it move along the track.
        this.gameObject.transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        

        if (LifeTime <= 0)
        {
            this.gameObject.transform.localScale -= (new Vector3(1, 0, 0) * Time.deltaTime * ScaleSpeed);
            this.gameObject.transform.localScale += (Vector3.up * Time.deltaTime * ScaleSpeed);
            if (this.gameObject.transform.localScale.x <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            Destroy(other.gameObject);
        }
    }
}
