using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string Load; //The name of the Scene that is to be loaded.
    public string Unload;
    public bool unloadsAsWell = false;

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
            //Has the Scene already been loaded?
            if (!SceneManager.GetSceneByName(Load).isLoaded)
            {
                SceneManager.LoadScene(Load, LoadSceneMode.Additive); //"Additive" is required for loading scenes with other scenes.
            }
            if (SceneManager.GetSceneByName(Unload).isLoaded && unloadsAsWell == true)
            {
                SceneManager.UnloadSceneAsync(Unload);
            }
        }
    }
}
