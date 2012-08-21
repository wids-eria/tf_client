Shader " Trails Forward/Tree"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Float) = 0.5
	}
	Category
	{
		Tags
		{
			"Queue" = "Geometry+200"
			"RenderType" = "TransparentCutout"
			"IgnoreProjector" = "True"
		}
	    SubShader
	    {
			Alphatest Greater [_Cutoff]
			AlphaToMask True
			
		    Pass
		    {
		    	ColorMaterial AmbientAndDiffuse
		        Lighting Off
		        SetTexture [_MainTex] { Combine texture * primary, texture * primary }
		    }
		}
	}
}