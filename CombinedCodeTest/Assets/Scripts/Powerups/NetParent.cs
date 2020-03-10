using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetParent : MonoBehaviour
{
    private GameObject Player;
    public GameObject[] AllNets;
    private NetParent lifetime;
    private MeshRenderer[] MR; //This object's own MR.
    private MeshRenderer[] OtherNetMR; //Another net's MR, if any.
    public float Lifetime = 12f; //This is a value that decreases by 1 every second. Once it hits 0, the object disappears.

    // Start is called before the first frame update
    void Start()
    {
        AllNets = GameObject.FindGameObjectsWithTag("Net"); //This is done as a check for multiple active net.
        MR = this.gameObject.GetComponentsInChildren<MeshRenderer>();

        //This handles for if a second net happens to be collected after another one is already active.
        //If this object is collected when there is already an active net...
        if (AllNets.Length > 1)
        {
            foreach (MeshRenderer mr in MR) 
            {
                mr.enabled = false; //...Then this one hides until it is needed.
            }
        }
           Player = GameObject.FindGameObjectWithTag("BikeInputCont"); //If it's the first/only net, then it finds the player, and continues like normal.
    }

    // Update is called once per frame
    void Update()
    {
        Lifetime -= Time.deltaTime; //The lifetime ticks down.
        this.gameObject.transform.position = Player.transform.position; //The net and its children track the player's position.
        if (Lifetime <= 0)
        {
            AllNets = GameObject.FindGameObjectsWithTag("Net"); //Another check for multiple nets is made.
            if (AllNets.Length > 1) //If there ARE multiples...
            {
                OtherNetMR = AllNets[1].GetComponentsInChildren<MeshRenderer>(); //...it finds the MeshRenderers of the next Net in the list...
                foreach (MeshRenderer mr in OtherNetMR)
                {
                    mr.enabled = true; //...and enables them.
                }
            }
            Destroy(this.gameObject); //The object's time has expired, so it disappears.
        }
    }
}
