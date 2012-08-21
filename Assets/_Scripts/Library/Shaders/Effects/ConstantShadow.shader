Shader " Library/Effects/Constant Shadow"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	Category
	{
		SubShader
		{
			Tags { "Queue" = "Overlay" }
			Blend DstColor Zero
			Lighting Off
			Pass
			{
				ColorMaterial AmbientAndDiffuse
				SetTexture [_MainTex]
				{
					constantColor [_Color]
				}
			}
		} 
	}
}
