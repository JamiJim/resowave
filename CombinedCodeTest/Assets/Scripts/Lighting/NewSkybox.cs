using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSkybox : MonoBehaviour
{

    //skyboxmaterial.SetFloat("_Blend", floatValue) <-----This is what is used to change the "blend" between the two skyboxes.

    public static float FadeSpeed = 0.3f;
    private float sbFade = 0; //This will be used to control how much the skybox fades between the first and second skybox. "0": Skybox 1 is fully opaque. "1": Skybox 2 is.

    public Material skyboxFade; //The fade material.

    public static int NextLevel = 0; //The ID, or order of the levels. It's numbered so that they can be changed nonspecifically.
    public GameObject LastLevelLights;

    private Light[] LastLevelLightsLights; //The lights of the LastLevelLights Game Object. Literally the "Last Level's Light Object's Lights."

    public LevelSkyboxes[] SkyboxList; //This is a group of skyboxes.

    // Start is called before the first frame update
    void Start()
    {
        //If the Level ID is at its end, it needs to cycle back to the start of the level list.
        if (SkyboxList.Length == NextLevel)
        {
            NextLevel = 0;
            Debug.Log("Resetting The Skybox Cycle!");
        }

        RenderSettings.skybox = skyboxFade; //The skybox fade begins.
        sbFade = 0; //In case it isn't sets the sbFade value to 0.
        skyboxFade.SetFloat("_Blend", sbFade); //This function sets the skybox to how much the skybox should blend between the two, from 0-1.

        LastLevelLights = GameObject.FindGameObjectWithTag("LevelLight");
        LastLevelLightsLights = LastLevelLights.GetComponentsInChildren<Light>();

        //Prepares the Fade Effect for the New Level's Skybox.
        skyboxFade.SetTexture("_FrontTex2", SkyboxList[NextLevel].FrontTex);
        skyboxFade.SetTexture("_BackTex2", SkyboxList[NextLevel].BackTex);
        skyboxFade.SetTexture("_UpTex2", SkyboxList[NextLevel].UpTex);
        skyboxFade.SetTexture("_DownTex2", SkyboxList[NextLevel].DownTex);
        skyboxFade.SetTexture("_LeftTex2", SkyboxList[NextLevel].LeftTex);
        skyboxFade.SetTexture("_RightTex2", SkyboxList[NextLevel].RightTex);

        //The lights used by the new skybox go here.
        Instantiate(SkyboxList[NextLevel].Lights, this.transform.position, this.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {

        //Fade out the old lights.
        foreach (Light light in LastLevelLightsLights)
        {
            light.intensity -= Time.deltaTime * FadeSpeed;
        }

        //This part here is likely redundant, as I realized that all the necessary options could've just been put in Start().
        /*if (sbFade == 0)
        {
            
        }*/

        sbFade += Time.deltaTime * FadeSpeed; //This is applied to the object constantly, until its deletion. It just controls the fading.
        skyboxFade.SetFloat("_Blend", sbFade); //This sets the fading to be constantly increasing, changing from the first skybox to the second skybox.

        //The Cubemap Transitions need to exist for this to work, so it won't do it if they don't exist.
        if (SkyboxList[NextLevel].NewLevelCubeMapTransition.Length > 0)
        {
            //Depending on how big NewLevelCubeMapTransition is, this is applied a different number of times.
            for (int i = 0; i < SkyboxList[NextLevel].NewLevelCubeMapTransition.Length; i++)
            {
                //This, in essence, creates the transitions from one cubemap to the next, and how many times it does it increases as more cubemaps are supplied.
                if (sbFade >= (i * 1.0f) / SkyboxList[NextLevel].NewLevelCubeMapTransition.Length && sbFade < (i + 1.0f) / SkyboxList[NextLevel].NewLevelCubeMapTransition.Length && sbFade < 1)
                {
                    RenderSettings.customReflection = SkyboxList[NextLevel].NewLevelCubeMapTransition[i]; //Then, set the cubemap to the one supplied at position 'i.'
                    //Debug.Log((i * 1.0f) / NewLevelCubeMapTransition.Length); //Use these if something for some reason goes wrong.
                    //Debug.Log(RenderSettings.customReflection);
                }
            }
        }

        //Once the new Skybox has fully faded in, the object that creates it isn't needed anymore. It performs a few more operations before it deletes the object.
        if (sbFade >= 1)
        {
            RenderSettings.customReflection = SkyboxList[NextLevel].FinalCubeMap; //Set the new level's cubemap.
            skyboxFade.SetTexture("_FrontTex", skyboxFade.GetTexture("_FrontTex2"));
            skyboxFade.SetTexture("_BackTex", skyboxFade.GetTexture("_BackTex2"));
            skyboxFade.SetTexture("_UpTex", skyboxFade.GetTexture("_UpTex2"));
            skyboxFade.SetTexture("_DownTex", skyboxFade.GetTexture("_DownTex2"));
            skyboxFade.SetTexture("_LeftTex", skyboxFade.GetTexture("_LeftTex2"));
            skyboxFade.SetTexture("_RightTex", skyboxFade.GetTexture("_RightTex2"));
            skyboxFade.SetFloat("_Blend", 0); //Sets up the skybox for the next transition, whenever that may be.
            NextLevel += 1; //Get the next level in the list set up for when the level changes again.
            Destroy(LastLevelLights); //With the old lights out of view, remove them from the scene.
            Destroy(this.gameObject); //With all that done, destroy the object.
        }
    }
}
