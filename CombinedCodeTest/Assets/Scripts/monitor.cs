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
    }
    void Update()
    {
        Vector3 temp = new Vector3(0, 0, 500f);
      
        CPUmonitor.transform.position = bike.transform.position+temp;
    }
}
