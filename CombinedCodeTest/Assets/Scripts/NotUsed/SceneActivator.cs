using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneActivator : MonoBehaviour
{
    public string Activate; //The name of the Scene that is to be activated.
    public string LevelToUnload; //The name of the Scene that is to be unloaded.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Activate));

            //Has this Scene been loaded yet?
            if (SceneManager.GetSceneByName(LevelToUnload).isLoaded)
            {
                SceneManager.UnloadSceneAsync(LevelToUnload);
            }
        }
    }
}