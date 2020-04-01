using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monitor : MonoBehaviour
{
    public GameObject bike;
    public GameObject CPUmonitor;
    float timer = 0.0f;
    int seconds;
    void Start() 
    {
        invokeRepeating("Rise",0f,30f);
        invokeRepeating("Fall", 20f, 30f);
    }
    void Update()
    {
        timer += Time.deltaTime%60;
        Debug.Log(timer+"seconds");
        Vector3 temp = new Vector3(-100, 50, 200f);
        CPUmonitor.transform.position = bike.transform.position+temp;
        
    }
    void Rise() { }
}
