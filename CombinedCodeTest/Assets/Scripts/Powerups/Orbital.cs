using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbital : MonoBehaviour
{

    private float Modifier;
    public float modSpeed = 1f; //How fast the orbit occurs.
    public float Distance = -4f; //The distance the orbit will travel.

    // Update is called once per frame
    void Update()
    {
        Modifier += Time.deltaTime * modSpeed;
        this.transform.Translate(Vector3.right * Time.deltaTime * Mathf.Cos(Modifier) * Distance);
        this.transform.Translate(Vector3.forward * Time.deltaTime * Mathf.Sin(Modifier) * Distance);
    }
}
