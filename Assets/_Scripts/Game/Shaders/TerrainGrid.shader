Shader "Trails Forward/Terrain Grid" {
	Properties {
		_MainTex ("Main", 2D) = "" {}
	}
	Subshader {
		Pass {
			ZWrite off
			//ColorMask RGB
			Blend One One
			SetTexture [_MainTex] {
				combine texture, ONE - texture
			}
		}
	}
}