// front-facing-only shader for 3D text in a HUD
Shader " Library/GUI/HUD Text"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TextColor ("Text Color (RGBA)", Color) = (1.0, 1.0, 1.0, 1.0)
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
			SetTexture [_MainTex]
			{
				constantColor [_TextColor]
				combine texture + constant, texture * constant
			}
		}
	}
}