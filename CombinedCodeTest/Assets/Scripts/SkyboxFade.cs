using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkyboxFade : MonoBehaviour
{

    public Color invisible;
    public float fadeSpeed;
    public bool isParent = false;
    public Image[] skybox; //This is used to get the values of the child objects.
    public static string SceneToBeLoaded; //This is adjusted by the script that loads in the skybox transition.
    public bool isFadingOut = false;
    public GameObject SceneUnloader;
    public static Scene SceneToUnload;

    // Start is called before the first frame update
    void Start()
    {
        SceneToUnload = (SceneManager.GetActiveScene());
        skybox = this.GetComponentsInChildren<Image>();
        if (this.GetComponentInChildren<Image>()) //If it can find a child, and the proper component inside the child, it knows it's a parent.
        {
            invisible = this.GetComponentInChildren<Image>().material.color;
            isParent = true;
        }
        else
        {
            invisible = this.GetComponent<Image>().material.color;
        }
        invisible.a = 0; //It's fading in, so the alpha should be set as zero.
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(SceneToBeLoaded)) //Is the scene already loaded and active?
        {
            SceneManager.LoadScene(SceneToBeLoaded, LoadSceneMode.Additive); //The scene is empty (it's only used for its light rendering and skybox data), so it won't be noticed until after it's loaded.
        }
        else
        {
            SceneToBeLoaded = "TransitionTest2";
            SceneManager.LoadScene("TransitionTest2", LoadSceneMode.Additive);
        }
    }

    
    private void Update()
    {
       
            //If it's a parent, sets the children's colors to the value that is about to be modified.
            if (isParent == true)
            {
                for (int i = 0; i < skybox.Length; i++)
                    skybox[i].material.color = invisible;
            }
            else
            {
                this.GetComponent<Image>().material.color = invisible; //If not, it just uses itself.
            }
            if (!isFadingOut)
            {
            invisible.a += Time.deltaTime * fadeSpeed; //Fades in using the speed of the fade given in the editor.
            }
            if (invisible.a >= 1 && isFadingOut == false) //Once it is fully faded in, it begins to fade out.
            {
                invisible.a = 0.999999f; //In case invisible.a is above 1 when this script is called for any reason.
                if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(SceneToBeLoaded))
                {
                    SceneToUnload = (SceneManager.GetActiveScene());
                    Instantiate(SceneUnloader, this.transform.position, this.transform.rotation);
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneToBeLoaded));
                }
                else
                {
                    
                }
                
                isFadingOut = true;
            }
            if (isFadingOut)
            {
                invisible.a -= Time.deltaTime * fadeSpeed;
            }
            if (invisible.a <= 0 && isFadingOut == true) //Once it cannot be seen, it is destroyed from the scene.
            {
                Destroy(this.gameObject);
            }
            
    }
}