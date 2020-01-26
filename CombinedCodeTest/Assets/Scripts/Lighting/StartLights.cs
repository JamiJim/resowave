using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLights : MonoBehaviour
{
    public Material skyboxFade; //The fade material.
    public Texture SolidBlackTexture; //The solid black texture, used at the start of the game.

    //Sets the Skybox to how it should look at the start of the game.
    private void Awake()
    {
        skyboxFade.SetTexture("_FrontTex", SolidBlackTexture);
        skyboxFade.SetTexture("_BackTex", SolidBlackTexture);
        skyboxFade.SetTexture("_UpTex", SolidBlackTexture);
        skyboxFade.SetTexture("_DownTex", SolidBlackTexture);
        skyboxFade.SetTexture("_LeftTex", SolidBlackTexture);
        skyboxFade.SetTexture("_RightTex", SolidBlackTexture);
    }

    void Start()
    {
        Destroy(this.gameObject);
    }
}
