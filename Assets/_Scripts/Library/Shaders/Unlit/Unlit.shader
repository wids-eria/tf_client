Shader " Library/Unlit/Textured"
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
                Lighting Off
                SetTexture [_MainTex] 
            }
        } 
    }
}