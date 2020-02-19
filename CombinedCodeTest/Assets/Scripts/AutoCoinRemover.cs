using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCoinRemover : MonoBehaviour
{
    private float DeletTime = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(DeletTime);
        DeletTime -= Time.deltaTime;

        if (DeletTime <= 0.0f)
        {
            Destroy(gameObject, 2);
            Debug.Log("Deleted");
        }
    }
}
