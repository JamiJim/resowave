using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{
    private float[] Intensity;
    private Light[] ThisLight;

    // Start is called before the first frame update
    void Start()
    {
        ThisLight = GetComponentsInChildren<Light>();
        Intensity = new float[ThisLight.Length];
  
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
            if (ThisLight[i].intensity < Intensity[i])
            {
                ThisLight[i].intensity += Time.deltaTime * NewSkybox.FadeSpeed;
            }

            if (ThisLight[i].intensity > Intensity[i])
            {
                for (int y = 0; y < ThisLight.Length; y++)
                    ThisLight[y].intensity = Intensity[y];
            }
        }
        
    }
}
