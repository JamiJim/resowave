using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BassGrenade : MonoBehaviour
{
    public float GrowthSpeed = 3;
    public int MaxSize = 30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale += new Vector3(1,1,1) * GrowthSpeed * Time.deltaTime;
        if (transform.localScale.x >= MaxSize) //Since everything scales uniformly, this works fine.
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            VZPlayer.Score += 1; //Need to implement a "Coin Collection" script that properly takes into account coin score amounts.
        }
    }
}
