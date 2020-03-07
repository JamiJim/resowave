using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatChange : MonoBehaviour
{
    public Material[] Mats;
    public Renderer rend;

    public Material Bike;
    public Material ViralBike;

    private void Start()
    {
      //Set Materials to original Bike Colors
      Mats = new Material[1];
      Mats = rend.GetComponent<Renderer>().materials;

        Mats[0] = Bike;
        Mats[1] = Bike;

      rend.GetComponent<Renderer>().materials = Mats;
    }
    
    //Once a Virus is picked up do this 
    public void ChangeMat()
    {
        //Change Bike to look like its infected
        Mats = new Material[1];
        Mats = rend.GetComponent<Renderer>().materials;

        Mats[0] = Bike;
        Mats[1] = ViralBike;

        rend.GetComponent<Renderer>().materials = Mats;
    }

    public void ReturnMat()
    {
        //Return Bike to Original Colors
        Mats = new Material[1];
        Mats = rend.GetComponent<Renderer>().materials;

        Mats[0] = Bike;
        Mats[1] = Bike;

        rend.GetComponent<Renderer>().materials = Mats;
    }

}
