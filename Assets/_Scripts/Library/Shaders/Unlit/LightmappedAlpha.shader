Shader " Library/Unlit/Light Mapped - Alpha"
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
			Tags {"Queue" = "Transparent"}
			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off
				ZTest Less
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
