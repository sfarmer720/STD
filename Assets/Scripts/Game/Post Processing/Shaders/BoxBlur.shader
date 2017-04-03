// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "STD/BoxBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		//Guassian Passes
		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			struct appdata{
				float4 vertex: POSITION;
				float2 uv: TEXCOORD0;
			};

			struct v2f{
				float4 pos: SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			v2f vert (appdata v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				#if UNITY_UV_STARTS_AT_TOP
					if(_MainTex_TexelSize.y < 0)
						o.uv.y = 1 - o.uv.y;
				#endif
				return o;
			}

			float4 BoxBlur(sampler2D tex, float2 uv, float4 size){
				float4 blur = 	tex2D( tex, uv + float2(	-size.x,	size.y	)) + 
								tex2D( tex, uv + float2(	0, 			size.y	)) + 
								tex2D( tex, uv + float2(	size.x, 	size.y	)) + 
								tex2D( tex, uv + float2(	-size.x, 	0		)) + 
								tex2D( tex, uv + float2(	0,			0		)) + 
								tex2D( tex, uv + float2(	size.x,		0		)) + 
								tex2D( tex, uv + float2(	-size.x,	-size.y	)) + 
								tex2D( tex, uv + float2(	0,			-size.y	)) + 
								tex2D( tex, uv + float2(	size.x,		-size.y	)) ;
				return blur / 9;
			}

			float4 frag(v2f i): SV_TARGET{
				return BoxBlur(_MainTex, i.uv, _MainTex_TexelSize);
			}

			ENDCG
		}
	}
}
