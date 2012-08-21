Shader " Library/Unlit/Textured - Vertex Colored Cutoff"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
    Category
    {
        SubShader
        {
			Alphatest Greater [_Cutoff]
			AlphaToMask True
			
            Pass
            {
				ColorMaterial AmbientAndDiffuse
                Lighting Off
                SetTexture [_MainTex] {Combine texture * primary}
            }
        } 
    }
}