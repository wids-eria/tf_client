Shader " Library/Unlit/Light Mapped - Alpha Cutoff"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightMap ("Lightmap (RGB)", 2D) = "lightmap" { LightmapMode }
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	Category
	{
		Alphatest Greater [_Cutoff]
		AlphaToMask True
		SubShader
		{
			Pass
			{
				Lighting Off
				BindChannels
				{
					Bind "Vertex", vertex
					Bind "texcoord", texcoord0
					Bind "texcoord1", texcoord1
				}
				SetTexture [_MainTex]
				{
					constantColor [_Color]
				}
				SetTexture [_LightMap]
				{
					combine previous * texture, previous
				}
			}
		} 
	}
}
