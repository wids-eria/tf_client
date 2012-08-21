Shader " Trails Forward/Terrain Highlighter" {
	Properties {
		_Zoning ("Zoning (RGB)", 2D) = "black" {}
		_Selection ("Selection (A)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Overlay" }
        Lighting Off
        ZWrite Off
        Pass
        {
        	Blend One One
        	CGPROGRAM
        	#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _Zoning;
			sampler2D _Selection;
			float4x4 _Projector;
			
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = mul (_Projector, v.vertex).xy;
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				half4 zone = tex2D(_Zoning, i.uv);
				half4 sel = tex2D(_Selection, i.uv);
				return half4(zone.r+sel.a, zone.g+sel.a, zone.b+sel.a, sel.a);
			}
			ENDCG
        }
	}
}
