Shader " Library/Vertex Color Only/Light Beams"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
    Category
    {
        SubShader
        {
	        Tags { "Queue" = "Overlay" }
	        Blend One One
            Lighting Off
            ZWrite Off
            Pass
            {
				ColorMaterial AmbientAndDiffuse
            }
        } 
    }
}
