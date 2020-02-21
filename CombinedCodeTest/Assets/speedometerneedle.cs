using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedometerneedle : MonoBehaviour
{
    public VZPlayer gm;
    public GameObject needle;
    public float needleSpeed;
    private float speed;
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
        speed = gm.Speed() * -10;
    }

    // Update is called once per frame
    void Update()
    {
        /*peed = gm.Speed();
         Debug.Log("Speed " + speed );*/
        if (speed < -90) { speed = -90; }
        else if (speed > -90)
        {
            speed = gm.Speed() * -10;
        }
            needle.transform.rotation=Quaternion.Euler(0,0,speed+50);
            Debug.Log("rotation:  "+needle.transform.rotation+" Speed: "+speed);
        
    }
}
