Shader " Library/Unlit/Textured - Decal"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Decal ("Decal (RGBA)", 2D) = "black" {}
	}
    Category
    {
        SubShader
        {
            Pass
            {
                Lighting Off
                SetTexture [_MainTex] {combine texture}
				SetTexture [_Decal] {combine texture lerp (texture) previous}
            }
        } 
    }
}