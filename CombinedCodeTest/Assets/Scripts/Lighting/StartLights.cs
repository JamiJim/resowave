﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLights : MonoBehaviour
{
    public Material skyboxFade; //The fade material.
    public Cubemap SolidBlackTexture; //The solid black texture, used at the start of the game.

    //Sets the Skybox to how it should look at the start of the game.
    private void Awake()
    {
        skyboxFade.SetTexture("_Tex1", SolidBlackTexture);
    }

    void Start()
    {
        Destroy(this.gameObject);
    }
}
