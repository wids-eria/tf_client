Shader " Library/Unlit/Textured - Alpha"
{
	Properties
	{
		_MainTex ("Base (RGBA)", 2D) = "black" {}
	}
    SubShader
    {
		Tags {"Queue" = "Transparent"}
	    Pass
	    {
			Blend SrcAlpha OneMinusSrcAlpha
	        Lighting Off
			ZWrite Off
			ZTest Less
	        SetTexture [_MainTex] {constantColor[_Color]}
	    }
	}
}