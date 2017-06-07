Shader "STD/AlphaMask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex, _AddTex;
			float4 _MainTex_ST,_MainTex_TexelSize,_AddTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = UnityObjectToClipPos( v.vertex );
				o.uv.xy = TRANSFORM_TEX( v.uv, _MainTex );

				#if UNITY_UV_STARTS_AT_TOP
					if(_MainTex_TexelSize.y < 0)
						o.uv.y = 1 - o.uv.y;
				#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

			//fixed4 tex1 = tex2D(_MainTex, float2(i.uv.x, 1-i.uv.y));
			fixed4 tex1 = tex2D(_MainTex, i.uv);
			fixed4 tex2 = tex2D(_AddTex, i.uv);

			//return tex2D(_AddTex, i.uv);

			return fixed4( (tex1.rgb * tex2.rgb), tex1.a);

			}
			ENDCG
		}
	}
}
