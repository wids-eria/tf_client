
Shader "Hidden/ClearExceptSkyboxShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	// Shader code pasted into all further CGPROGRAM blocks	
	#LINE 34
 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 5 to 5
//   d3d9 - ALU: 5 to 5
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"!!ARBvp1.0
# 5 ALU
PARAM c[5] = { program.local[0],
		state.matrix.mvp };
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 5 instructions, 0 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mov oT0.xy, v1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _ZBufferParams;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  highp float d;
  lowp float tmpvar_1;
  tmpvar_1 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x;
  d = tmpvar_1;
  highp float tmpvar_2;
  tmpvar_2 = (1.0/(((_ZBufferParams.x * d) + _ZBufferParams.y)));
  d = tmpvar_2;
  highp float x;
  x = (0.99 - tmpvar_2);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _ZBufferParams;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  highp float d;
  lowp float tmpvar_1;
  tmpvar_1 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x;
  d = tmpvar_1;
  highp float tmpvar_2;
  tmpvar_2 = (1.0/(((_ZBufferParams.x * d) + _ZBufferParams.y)));
  d = tmpvar_2;
  highp float x;
  x = (0.99 - tmpvar_2);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"agal_vs
[bc]
aaaaaaaaaaaaadaeadaaaaoeaaaaaaaaaaaaaaaaaaaaaaaa mov v0.xy, a3
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 6 to 6, TEX: 1 to 1
//   d3d9 - ALU: 7 to 7, TEX: 2 to 2
SubProgram "opengl " {
Keywords { }
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D
"!!ARBfp1.0
# 6 ALU, 1 TEX
PARAM c[2] = { program.local[0],
		{ 0, 0.99000001 } };
TEMP R0;
TEX R0.x, fragment.texcoord[0], texture[0], 2D;
MAD R0.x, R0, c[0], c[0].y;
RCP R0.x, R0.x;
SLT R0.x, -R0, -c[1].y;
MOV result.color, c[1].x;
KIL -R0.x;
END
# 6 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D
"ps_2_0
; 7 ALU, 2 TEX
dcl_2d s0
def c1, 0.99000001, 0.00000000, 1.00000000, 0
dcl t0.xy
texld r0, t0, s0
mad r0.x, r0, c0, c0.y
rcp r0.x, r0.x
add r0.x, -r0, c1
cmp r0.x, r0, c1.y, c1.z
mov_pp r0, -r0.x
texkill r0.xyzw
mov_pp r0, c1.y
mov_pp oC0, r0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "flash " {
Keywords { }
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D
"agal_ps
c1 0.99 0.0 1.0 0.0
[bc]
ciaaaaaaaaaaapacaaaaaaoeaeaaaaaaaaaaaaaaafaababb tex r0, v0, s0 <2d wrap linear point>
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaoeabaaaaaa mul r0.x, r0.x, c0
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaffabaaaaaa add r0.x, r0.x, c0.y
afaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rcp r0.x, r0.x
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaabaaaaoeabaaaaaa add r0.x, r0.x, c1
cjaaaaaaaaaaabacaaaaaaaaacaaaaaaabaaaaffabaaaaaa sge r0.x, r0.x, c1.y
chaaaaaaaaaaaaaaaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa kil a0.none, r0.x
aaaaaaaaaaaaapacabaaaaffabaaaaaaaaaaaaaaaaaaaaaa mov r0, c1.y
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

}

#LINE 44

  }
}

Fallback off
	
} // shader