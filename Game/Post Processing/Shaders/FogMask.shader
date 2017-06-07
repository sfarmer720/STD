// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "STD/FogMask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_AddTex ("Texture", 2D) = "black" {}

	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex, _AddTex;//, _Scene;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

			fixed4 tex1 = tex2D(_MainTex, i.uv);
			fixed4 tex2 = tex2D(_AddTex, i.uv) * float4(0.5,0.5,0.5,1);

			return tex1 + tex2;

			return fixed4( tex1.rgb + (tex2.rgb * 0.5), tex1.a);
			//return tex1 + (tex2 * 0.5)

			}
			ENDCG
		}
	}
}
