Shader " Library/Unlit/Textured - Vertex Colored with Alpha"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
    Category
    {
        SubShader
        {
			ZWrite Off
			Alphatest Greater 0
			Tags { Queue=Transparent }
			Blend SrcAlpha OneMinusSrcAlpha
            Pass
            {
				ColorMaterial AmbientAndDiffuse
                Lighting Off
				SetTexture [_MainTex] { Combine texture * primary, texture * primary }
            }
        } 
    }
}