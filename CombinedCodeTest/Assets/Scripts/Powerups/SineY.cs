using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineY : MonoBehaviour
{
    public float HeightMinMax = 3;
    public float modSpeed = 8;
    private float sinModifier = 0;

    // Update is called once per frame
    void Update()
    {
        sinModifier += Time.deltaTime * modSpeed;
        this.transform.Translate(Vector3.up * Time.deltaTime * HeightMinMax * Mathf.Sin(sinModifier));
    }
}
