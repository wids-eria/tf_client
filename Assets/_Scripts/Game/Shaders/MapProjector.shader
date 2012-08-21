Shader "Trails Forward/MapProjector" {
	Properties {
		_MainTex ("Main (RGBA)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Overlay" }
        Lighting Off
        ZWrite Off
        Pass
        {
        	Blend SrcAlpha OneMinusSrcAlpha
        	CGPROGRAM
        	#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
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
				half4 main = tex2D(_MainTex, i.uv);
				return half4(main.r, main.g, main.b, main.a);
			}
			ENDCG
        }
	}
}

