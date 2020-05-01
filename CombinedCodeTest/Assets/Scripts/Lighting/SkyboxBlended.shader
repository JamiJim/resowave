//"Skybox Blended" Shader: Modification of File Created by Aras Pranckevicius
//File Author: Aras Pranckevicius
//Content is used from, modified from, and available under the Creative Commons Attribution Share Alike license (https://creativecommons.org/licenses/by-sa/3.0/)

Shader "Skybox/Blended" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    _Blend ("Blend", Range(0.0,1.0)) = 0.5
	_Tex1 ("Current Skybox", Cube) = "white" {}
    _Tex2 ("Next Skybox", Cube) = "white" {}
}

SubShader {
    Tags { "Queue" = "Background" }
    Cull Off
    Fog { Mode Off }
    Lighting Off
    Color [_Tint]
    Pass {
        SetTexture [_Tex1] { combine texture }
        SetTexture [_Tex2] { constantColor (0,0,0,[_Blend]) combine texture lerp(constant) previous }
    }
}

Fallback "Skybox/Cubemap", 1
}