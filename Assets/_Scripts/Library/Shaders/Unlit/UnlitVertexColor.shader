Shader " Library/Unlit/Textured - Vertex Colored"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
    Category
    {
        SubShader
        {
            Pass
            {
				ColorMaterial AmbientAndDiffuse
                Lighting Off
                SetTexture [_MainTex] {Combine texture * primary}
            }
        } 
    }
}