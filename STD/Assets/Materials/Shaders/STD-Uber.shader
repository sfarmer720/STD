Shader "STD/STD-Uber"
{

	//Properties
	Properties{

		//Albedo ( Main Tex & Tint )
		_Tint ( "Tint", Color) = ( 1, 1, 1, 1 ) 
		_MainTex ( "Albedo", 2D) = "white"{}

		//Normal Map
		_Normal ( "Normal Map", 2D) = "bump"{}
		_NormalScale ("Normal Intensity", Float) = 1

		//Bump Map
		_Bump ( "Bump Map", 2D ) = "grey"{}
		_BumpNormal ( "Bump Map Normals", 2D) = "bump"{}
		_BumpScale ( "Bump Intensity", Float) = 1
		_BumpMask ( "Bump Mask", 2D ) = "white"{}

		//Metallic
		_MetalMap ( "Metallic", 2D) = "white" {}
[Gamma]	_Metal ( "Metallic", Range( 0, 1)) = 0

		//Emission
		_EmissionMap ( "Emission", 2D) = "black"{}
		_Emission ("Emission", Color) = ( 0, 0, 0)

		//Occlusion
		_OccMap ("Occlusion Map", 2D) = "white"{}
		_OccIntensity ("Occlusion Intensity", Range( 0, 1)) = 1

		//Smoothness
		_Smooth ( "Smoothness", Range( 0, 1 )) = 0.5

		//Alpha Cutoff
		_AlphaCut ( "Alpha Cuttoff", Range( 0, 1)) = 0.5

		//Outline
		_Outline ( "Outline Color", Color) = ( 0, 0, 0, 1 )
		_OutlineSize ( "Outline Size", Range( 0.002, 0.3)) = 0.005

		//Pass Variables
		_SrcBlend ("SrcBlend", Float) = 1
		_DstBlend ("DstBlend", Float) = 0
		_ZWrite ("ZWrite", Float) = 1
	}

	SubShader{

	//Main Light Pass
		Pass{

			Tags{
				"LightMode" = "ForwardBase"
			}

			//Set blend modes from inspector selections
			Blend [_SrcBlend] [_DstBlend]
			Zwrite [_ZWrite]

			CGPROGRAM

			//define compiler versions to support all lights, vertex lights, and shadows
			#pragma multi_compile_fwdadd_fullShadows
			#pragma multi_compile _ VERTEXLIGHT_ON
			#pragma multi_compile _ SHADOWS_SCREEN

			//Define Rendering modes
			#pragma shader_feature _ _RENDER_CUT _RENDER_FADE _RENDER_TRANS

			//Define custom keywords
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _BUMP_MAP
			#pragma shader_feature _BUMP_MAP_NORMAL
			#pragma shader_feature _BUMP_MASK
			#pragma shader_feature _METAL_MAP
			#pragma shader_feature _ _SMOOTH_ALBEDO _SMOOTH_METAL
			#pragma shader_feature _OCC_MAP
			#pragma shader_feature _EMISSION_MAP

			//define Vertex and Frag functions, target best BRDF
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag


			//Include & define pass
			#define FORWARD_BASE_PASS
			#include "LightPass.cginc"

			ENDCG
		}

	//Additional Light Pass
		Pass{

			Tags{
				"LightMode" = "ForwardAdd"
			}

			//Set blend modes from inspector selections
			Blend [_SrcBlend] [_DstBlend]
			Zwrite off

			CGPROGRAM

			//define compiler versions
			#pragma multi_compile_fwdadd_fullShadows
			#pragma multi_compile _ VERTEXLIGHT_ON

			//Define Rendering modes
			#pragma shader_feature _ _RENDER_CUT _RENDER_FADE _RENDER_TRANS

			//Define custom keywords
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _BUMP_MAP
			#pragma shader_feature _BUMP_MAP_NORMAL
			#pragma shader_feature _BUMP_MASK
			#pragma shader_feature _METAL_MAP
			#pragma shader_feature _ _SMOOTH_ALBEDO _SMOOTH_METAL

			//define Vertex and Frag functions, target best BRDF
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag


			//Include & define pass
			#include "LightPass.cginc"

			ENDCG
		}

	//Shadow Pass
		Pass{

			Tags{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM

			//define shadows multicompile
			#pragma multi_compile_shadowcaster

			//Define Rendering modes
			#pragma shader_feature _ _RENDER_CUT _RENDER_FADE _RENDER_TRANS

			//Define custom keywords
			#pragma shader_feature _SMOOTH_ALBED
			#pragma shader_feature _SEMITRANS_SHADOWS

			//define Vertex and Frag functions, target best BRDF
			#pragma target 3.0
			#pragma vertex shadowVert
			#pragma fragment shadowFrag

			//Include & define pass
			#include "ShadowPass.cginc"

			ENDCG
		}
	}

	//Set Editor to use
	CustomEditor "STDMatEditor"
}
