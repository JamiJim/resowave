using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monitor : MonoBehaviour
{
    public GameObject bike;
    public GameObject CPUmonitor;
    public bool rising;
    float timer = 0.0f;
    float i;
    void Start() 
    {
        InvokeRepeating("Rise",0f,30f);
        InvokeRepeating("Fall", 10f, 30f);
    }
    void Update()
    {
        Vector3 temp = new Vector3(-100, i, 200f);
        switch (rising) 
        {
            case (true):
                if (i < 50) 
                {
                    i= i + 0.1f;
                }
                break;
            case (false):
                if(i>-50)
                i=i - 0.1f;
                break;
        }
        CPUmonitor.transform.position = bike.transform.position+temp;
    }
    void Rise() 
    {
        rising = true;
    }
    void Fall()
    {
        rising = false;
    }
}
