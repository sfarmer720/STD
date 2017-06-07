﻿Shader "STD/Stencil"
{

	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue" = "Geometry-1"}

		Colormask 0
		Zwrite off

		Stencil{
			Ref 1
			Comp always
			Pass replace
		}

		Pass
		{

			Cull Back
			ZTest Less
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{

				//return half4(1,1,0,0);
				return 0;
			}
			ENDCG


		}
	}
}
