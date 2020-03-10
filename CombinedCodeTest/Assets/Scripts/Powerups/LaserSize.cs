using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSize : MonoBehaviour
{
    public float MaxSize = 1f;
    private bool ScaledUp = false;
    public float Lifetime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ScaledUp == false)
        {
            this.transform.localScale += (new Vector3(1, 0, 1) * Time.deltaTime);
        }
        if (this.transform.localScale.x >= MaxSize)
        {
            ScaledUp = true;
        }
        if (ScaledUp == true)
        {
            Lifetime -= Time.deltaTime;
            if (Lifetime <= 0)
            {
                this.transform.localScale -= (new Vector3(1, 0, 1) * Time.deltaTime);
            }
            if (this.gameObject.transform.localScale.x <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
