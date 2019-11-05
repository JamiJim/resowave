using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTransition : MonoBehaviour
{
    public GameObject SkyboxFader; //The skybox transition object should be selected.
    public string SceneToLoad; //Specify the name of the scene you're going to load, and eventually activate once the the skybox begins;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Instantiate(SkyboxFader, this.transform.position, this.transform.rotation);
            SkyboxFade.SceneToBeLoaded = SceneToLoad; //Let's the skybox object's script know that this is the scene that will be activated.
        }
    }
}

