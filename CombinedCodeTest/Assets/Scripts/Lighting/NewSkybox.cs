using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSkybox : MonoBehaviour
{

    //skyboxmaterial.SetFloat("_Blend", floatValue) <-----This is what is used to change the "blend" between the two skyboxes.

    public float FadeSpeed = 0.3f;
    private float sbFade = 0; //This will be used to control how much the skybox fades between the first and second skybox. "0": Skybox 1 is fully opaque. "1": Skybox 2 is.
    
    public Material skyboxFade; //The fade material.

    public Cubemap[] NewLevelCubeMapTransition; //Specify the number of Cubemap Transitions in the editor, then supply them. The functions below do the rest!
    public Cubemap NewLevelCubeMap; //The cubemap to be faded in.
    public Material newSkybox; //The new level's skybox.

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = skyboxFade; //The skybox fade begins.
        sbFade = 0; //In case it isn't sets the sbFade value to 0.
        skyboxFade.SetFloat("_Blend", sbFade); //This function sets the skybox to how much the skybox should blend between the two, from 0-1.

        //Once new levels are added, there will likely need to be another script or function that sets "NewLevelCubeMap," and all of the transitions,
        //depending on what level they're currently in, or whatever factor we go with.
    }
    
    // Update is called once per frame
    void Update()
    {
        
        sbFade += Time.deltaTime * FadeSpeed; //This is applied to the object constantly, until its deletion. It just controls the fading.
        skyboxFade.SetFloat("_Blend", sbFade); //This sets the fading to be constantly increasing, changing from the first skybox to the second skybox.

        //The Cubemap Transitions need to exist for this to work, so it won't do it if they don't exist.
        if (NewLevelCubeMapTransition.Length > 0)
        {
            //Depending on how big NewLevelCubeMapTransition is, this is applied a different number of times.
            for (int i = 0; i < NewLevelCubeMapTransition.Length; i++)
            {
                //This, in essence, creates the transitions from one cubemap to the next, and how many times it does it increases as more cubemaps are supplied.
                if (sbFade >= (i * 1.0f) / NewLevelCubeMapTransition.Length && sbFade < (i + 1.0f) / NewLevelCubeMapTransition.Length && sbFade < 1)
                {
                    RenderSettings.customReflection = NewLevelCubeMapTransition[i]; //Then, set the cubemap to the one supplied at position 'i.'
                    //Debug.Log((i * 1.0f) / NewLevelCubeMapTransition.Length); //Use these if something for some reason goes wrong.
                    //Debug.Log(RenderSettings.customReflection);
                }
            }
        }
        
        //Once the new Skybox has fully faded in, the object that creates it isn't needed anymore. It performs a few more operations before it deletes the object.
        if (sbFade >= 1)
        {
            RenderSettings.customReflection = NewLevelCubeMap; //Set the new level's cubemap.
            RenderSettings.skybox = newSkybox; //Set the new level's skybox.

            //Prepare the new skybox for the next transition.
            skyboxFade.SetFloat("_Blend", 1);
            {
                skyboxFade.SetTexture("_FrontTex", skyboxFade.GetTexture("_FrontTex2"));
                skyboxFade.SetTexture("_BackTex", skyboxFade.GetTexture("_BackTex2"));
                skyboxFade.SetTexture("_UpTex", skyboxFade.GetTexture("_UpTex2"));
                skyboxFade.SetTexture("_DownTex", skyboxFade.GetTexture("_DownTex2"));
                skyboxFade.SetTexture("_LeftTex", skyboxFade.GetTexture("_LeftTex2"));
                skyboxFade.SetTexture("_RightTex", skyboxFade.GetTexture("_RightTex2"));
            }
            
            Destroy(this.gameObject); //With all that done, destroy the object.
        }
    }
}
