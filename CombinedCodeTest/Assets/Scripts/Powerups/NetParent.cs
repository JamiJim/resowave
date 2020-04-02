using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetParent : MonoBehaviour
{
    private GameObject Player;
    public GameObject[] AllNets;
    public float Lifetime = 12f; //How long the object should last before it disappears.
    public string NetSide; //This should be whichever tag is used ("Left" or "Right").
    private float lifetime; //This is a value that decreases by 1 every second. Once it hits 0, the object disappears.
    public float ScaleSpeed = 2.2f;
    private bool isMaxSized = false;
    private bool HitMax = false;
    public Transform[] ChildNets;

    //Scales all of the net's children. Written as a function because it happens a LOT.
    void ScaleChildren(Vector3 Scale)
    {
        foreach (Transform Net in ChildNets)
        {
            Net.transform.localScale += Scale;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AllNets = GameObject.FindGameObjectsWithTag(NetSide + "Net"); //This is done as a check for multiple active net.
        ChildNets = GetComponentsInChildren<Transform>();
        //This handles for if a second net happens to be collected after another one is already active.
        //If this object is collected when there is already an active net...
        if (AllNets.Length > 1)
        {
            Destroy(AllNets[0].gameObject); //The old one is removed.
            isMaxSized = true;
            HitMax = true;
        }
        else
        {
            ScaleChildren(new Vector3(0, this.transform.localScale.y * -1, 0));
        }
        lifetime = Lifetime;
        Player = GameObject.FindGameObjectWithTag("BikeInputCont"); //If it's the first/only net, then it finds the player, and continues like normal.
    }

    // Update is called once per frame
    void Update()
    {
        if (isMaxSized == false)
        {
           ScaleChildren(Vector3.up * ScaleSpeed * Time.deltaTime);
        }
        if (ChildNets[0].transform.localScale.y >= 1 && HitMax == false)
        {
            Debug.Log(ChildNets[0].transform.localScale.y);
            isMaxSized = true;
            ScaleChildren(new Vector3(0, this.transform.localScale.y * -1, 0));
            ScaleChildren(new Vector3(0, 1, 0));
            HitMax = true;
        }
        lifetime -= Time.deltaTime; //The lifetime ticks down.
        this.gameObject.transform.position = Player.transform.position; //The net and its children track the player's position.
        if (lifetime <= 0)
        {
            ScaleChildren(Vector3.up * ScaleSpeed * Time.deltaTime * -1.0f); //Begin to shrink and disappear.
            if (ChildNets[0].transform.localScale.y <= 0)
            {
                Destroy(this.gameObject); //The object's time has expired, so it disappears.
            }
        }
    }
}
