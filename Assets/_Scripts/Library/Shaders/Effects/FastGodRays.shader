Shader " Library/Effects/Fast God Rays"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Tint ("Tint (RGB)", Color) = (1.0, 1.0, 1.0)
	}
    Category
    {
        SubShader
        {
	        Tags { "Queue" = "Overlay" }
	        Blend One One
            Lighting Off
            ZWrite Off
            Cull Off
            Pass
            {
				ColorMaterial AmbientAndDiffuse
				SetTexture [_MainTex]
				{
					Combine texture * primary
				}
				SetTexture [_MainTex]
				{
					constantColor [_Tint]
					combine previous * constant
				}
            }
        } 
    }
}
