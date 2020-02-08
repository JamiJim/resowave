using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCoinRemover : MonoBehaviour
{
    private float DeletTime = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Delete", 15f);
    }

    // Update is called once per frame
    void Delete()
    {
        for(int i = 0; i < 16; i++)
        {
            DeletTime -= i;
        }
        

        if (DeletTime <= 0.0f)
        {
            Destroy(gameObject, 20);
            //Debug.Log("Deleted");
        }
    }
}
