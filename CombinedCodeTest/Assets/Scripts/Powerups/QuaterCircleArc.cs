using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaterCircleArc : MonoBehaviour
{
    private float Modifier;
    private float Mod2;
    public float modSpeedY = 12f; //How fast the arc occurs (Y-axis).
    public float modSpeedZ = 12f; //How fast the arc occurs (Z-axis).
    public float DistanceY = -48f; //The distance the arc will travel (Y-axis).
    public float DistanceZ = -48f; //The distance the arc will travel (Z-axis).
    private float Lifetime = 0;
    public float TargetTime = 1.570796f; //The time before the object spawns the effect.
    public GameObject Effect;

    // Update is called once per frame
    void Update()
    {
        Lifetime += Time.deltaTime;
        Modifier += Time.deltaTime * modSpeedY;
        Mod2 += Time.deltaTime * modSpeedZ;
        this.transform.Translate(Vector3.up * Time.deltaTime * Mathf.Cos(Modifier) * DistanceY);
        this.transform.Translate(Vector3.forward * Time.deltaTime * Mathf.Sin(Mod2) * DistanceZ);
        if (Lifetime >= TargetTime) //After half of Pi seconds, the circle should have reached the peak of the arc, if both modifiers are set to 1.
        {
            Instantiate(Effect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
