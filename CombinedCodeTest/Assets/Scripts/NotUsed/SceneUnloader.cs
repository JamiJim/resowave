using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUnloader : MonoBehaviour
{
    private Scene SceneToUnload;

    // Start is called before the first frame update
    void Start()
    {
        //SceneToUnload = SkyboxFade.SceneToUnload;
        //Invoke("UnloadScene", 150f);
        Invoke("UnloadScene", 40f);
    }
    void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(SceneToUnload);
    }
}
