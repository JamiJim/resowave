using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSkyboxes : MonoBehaviour
{
    public string LevelName; //The name of the level, for ease of identification.
    public Texture FrontTex;
    public Texture BackTex;
    public Texture UpTex;
    public Texture DownTex;
    public Texture LeftTex;
    public Texture RightTex;
    public Cubemap[] NewLevelCubeMapTransition; //Specify the number of Cubemap Transitions in the editor, then supply them.
    public Cubemap FinalCubeMap; //Specify the cubemap to be used in the next skybox.
    public GameObject Lights; //The Lights used by this skybox.

}
