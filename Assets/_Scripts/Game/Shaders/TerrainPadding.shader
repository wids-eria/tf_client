Shader " Trails Forward/Terrain Padding" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Grid ("Grid (A)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 150

		CGPROGRAM
		#pragma surface surf Lambert
		
		sampler2D _MainTex;
		sampler2D _Grid;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_Grid;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb + tex2D(_Grid, IN.uv_Grid).a;
			o.Alpha = c.a;
		}
		ENDCG
	}
}