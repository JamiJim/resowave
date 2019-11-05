using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{

    public GameObject Fade;
    public Color invisible;
    public float fadeSpeed;
    public bool isFading = false;
    public bool isParent = false;
    bool lastLoop = false;
    public MeshRenderer[] mr; //This is used to store all the child objects'

    // Start is called before the first frame update
    void Start()
    {
        

        mr = Fade.GetComponentsInChildren<MeshRenderer>();

        if (Fade.GetComponentInChildren<MeshRenderer>())
        {
            invisible = Fade.GetComponentInChildren<MeshRenderer>().material.color;
            isParent = true;
        }else
            invisible = Fade.GetComponent<MeshRenderer>().material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isFading = true;
        }
    }

    private void Update()
    {
        if (isFading == true && invisible.a >= 0)
        {
            if (isParent == true)
            {
                for (int i = 0; i < mr.Length; i++)
                mr[i].material.color = invisible;
            }
            else
            {
                Fade.GetComponent<MeshRenderer>().material.color = invisible;
            }
            invisible.a -= Time.deltaTime * fadeSpeed;
            if (invisible.a < 0)
            {
                invisible.a = 0;
            }
            //Debug.Log(invisible.a);
        }
        if(lastLoop==true)
        {
            isFading = false;
        }
        if (invisible.a == 0)
        {
            lastLoop = true;
        }
    }
}
