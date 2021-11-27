// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "TextMeshPro/Mobile/Distance Field SSD" {

Properties {
	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_Outlineidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	[HDR]_UnderlayColor		("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_eightNormal		("eight Normal", float) = 0
	_eightBold			("eight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_Textureidth		("Texture idth", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0
	_MaskTex			("Mask Texture", 2D) = "white" {}
	_MaskInverse		("Inverse", float) = 0
	_MaskEdgeColor		("Edge Color", Color) = (1,1,1,1)
	_MaskEdgeSoftness	("Edge Softness", Range(0, 1)) = 0.01
	_MaskipeControl	("ipe Position", Range(0, 1)) = 0.5

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilriteMask	("Stencil rite Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

    _CullMode           ("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags {
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		riteMask [_StencilriteMask]
	}

	Cull [_CullMode]
	Zrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"

		#include "TMPro_Mobile.cginc"

		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
