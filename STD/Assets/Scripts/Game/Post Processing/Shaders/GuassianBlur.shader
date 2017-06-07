Shader "STD/GuassianBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
	}

	CGINCLUDE

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;


			//9-tap linear gaussian blur
			half4 GaussianFilter(float2 uv, float2 stride){

				//sample texture
				half4 s = tex2D( _MainTex, uv) * 0.227027027;

				//down sample 1
				float2 d1 = stride * 1.3846153846;
				s += tex2D( _MainTex, uv + d1) * 0.3162162162;
				s += tex2D( _MainTex, uv - d1) * 0.3162162162;

				//down sample 2
				float2 d2 = stride * 3.2307692308;
				s += tex2D( _MainTex, uv + d2) * 0.0702702703;
				s += tex2D( _MainTex, uv - d2) * 0.0702702703;

				return s;
			}

			//Quarter downsample
			half4 frag_quarter(v2f_img i): SV_TARGET{

				float4 d = _MainTex_TexelSize.xyxy * float4(1,1,-1,-1);
				half4 s = tex2D(_MainTex, i.uv + d.xy);
				s += tex2D(_MainTex, i.uv + d.xw);
				s += tex2D(_MainTex, i.uv + d.zy);
				s += tex2D(_MainTex, i.uv + d.zw);

				return s * 0.25;
			}

			//Standalone Blur filters
			half4 frag_blur_h(v2f_img i): SV_TARGET{
				return GaussianFilter(i.uv, float2(_MainTex_TexelSize.x, 0));
			}
			half4 frag_blur_v(v2f_img i): SV_TARGET{
				return GaussianFilter(i.uv, float2(0,_MainTex_TexelSize.y));
			}

	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		//Guassian Passes
		Pass{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_quarter
			ENDCG
		}
		Pass{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_blur_h
			#pragma target 3.0
			ENDCG
		}
		Pass{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_blur_v
			#pragma target 3.0
			ENDCG
		}
	}
}
