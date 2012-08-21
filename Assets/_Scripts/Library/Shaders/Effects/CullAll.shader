Shader " Library/Effects/Cull All"
{
	SubShader
	{
		Tags {"Queue" = "Background"}
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		ZWrite On
		ZTest Always
		Pass
		{
			Color(0,0,0,0)
		}
	}
}
