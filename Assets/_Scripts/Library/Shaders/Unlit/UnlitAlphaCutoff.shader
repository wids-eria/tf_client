Shader " Library/Unlit/Textured - Alpha Cutoff"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Float) = 0.5
	}
    SubShader
    {
		Alphatest Greater [_Cutoff]
		AlphaToMask True
		
	    Pass
	    {
	        Lighting Off
	        SetTexture [_MainTex] {constantColor[_Color]}
	    }
	}
}