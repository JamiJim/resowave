using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntivirusAttack : MonoBehaviour
{
    public float Speed;
    private Vector3 target;
    private GameObject[] Virus;
    private int ListDeduction = 1;

    //Find a Virus, and set that as the virus to target.
    private void TargetVirus()
    {
        if (GameObject.FindGameObjectWithTag("Virus"))
        {
            Virus = GameObject.FindGameObjectsWithTag("Virus"); //The virus being targeted.
            target = Virus[Virus.Length - ListDeduction].transform.position; //Since the objects loaded first are FOUND last, this whole process will need to be done from the back of the list, working to the front.
        }
        else
        {
            Destroy(this.gameObject); //If it cannot find any/another virus to attack, it is done.
        }
    }


    private void Start()
    {
        TargetVirus();
    }

    private void Update()
    {
        this.gameObject.transform.position = (Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * Speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            Destroy(other.gameObject);
            ListDeduction += 1;
            if (ListDeduction == (Virus.Length + 1))
            {
                Destroy(this.gameObject); //If there are no more viruses to target, destroy this object.
            }
            else
            {
                target = Virus[Virus.Length - ListDeduction].transform.position; //Otherwise, move to the next item in the list, working backwards.
            }
        }
    }
}
