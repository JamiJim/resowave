using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTerrain : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject,0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
