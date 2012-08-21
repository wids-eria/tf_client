Shader " Library/Unlit/Light Mapped"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightMap ("Lightmap (RGB)", 2D) = "lightmap" { LightmapMode }
	}
	Category
	{
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
					combine previous * texture
				}
			}
		} 
	}
}
