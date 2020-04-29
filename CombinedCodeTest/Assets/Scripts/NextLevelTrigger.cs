using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public GameObject NewSkybox;
    private VZPlayer source;
    private bool BeginTransition = false;

    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            if (BeginTransition == false)
            {
                Instantiate(NewSkybox, this.gameObject.transform.position, this.gameObject.transform.rotation); //Begins the skybox transition.

                if (source.levelNo == 2)
                {
                    source.levelNo = 0;
                }
                else
                {
                    source.levelNo++;
                }

                BeginTransition = true; //This is necessary, or else the objects will keep appearing.
            }
            Destroy(this.gameObject);
        }
    }
}
