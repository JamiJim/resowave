using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{
    private float[] Intensity;
    private Light[] ThisLight;
    private bool hasFadedIn = false;
    private float maxPossibleIntensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        ThisLight = GetComponentsInChildren<Light>();
        Intensity = new float[ThisLight.Length];
  
        //This gets the intensity values for the lights set in the editor, and sets them as the value that they're to be faded into.
        for (int i = 0; i < ThisLight.Length; i++)
        {
            Intensity[i] = ThisLight[i].intensity;
        }

        foreach (Light light in ThisLight)
        {
            light.intensity = 0;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ThisLight.Length; i++)
        {
            if (ThisLight[i].intensity < Intensity[i] && hasFadedIn == false)
            {
                ThisLight[i].intensity += Time.deltaTime * NewSkybox.FadeSpeed;
            }

            if (ThisLight[i].intensity > Intensity[i] && hasFadedIn == false)
            {
               ThisLight[i].intensity = Intensity[i];
            }
            
        }
        //At most, the Intensity is going to be 1. So, by the time this value hits 1, using the same fade speed as the lights, this means that every light should have faded in.
        //This has to be checked to make sure that the lights can fade out in the transition to the next level.
        maxPossibleIntensity += Time.deltaTime * NewSkybox.FadeSpeed;
        if (maxPossibleIntensity >= 1)
        {
            hasFadedIn = true;
        }

    }
}
