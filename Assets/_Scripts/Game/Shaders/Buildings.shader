Shader " Trails Forward/Buildings"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
    Category
    {
    	Tags
    	{
    		"RenderType" = "Opaque"
    		"IgnoreProjector" = "True"
    	}
        SubShader
        {
            Pass
            {
				ColorMaterial AmbientAndDiffuse
                Lighting Off
                SetTexture [_MainTex] { Combine texture * primary }
            }
        } 
    }
}