Shader "STD/StencilMask"
{
	Properties
	{
		_Color ("Color", Color) = (0,0,0,0)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True"}
		LOD 200

		Stencil{
			Ref 1
			Comp notequal
			Pass keep
		}

		CGPROGRAM
		#pragma surface surf Lambert alpha
		sampler2D _MainTex;
		float4 _Color;

		struct Input{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o){
			fixed4 c = tex2D( _MainTex, IN.uv_MainTex )*_Color;
			o.Albedo = c.rgb;
			o.Alpha = 1 - c.a;
		}


		ENDCG
	}
}
