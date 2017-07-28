Shader "STD/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scene ("Texture", 2D) = "white" {}
		_Mask ("Texture", 2D) = "white" {}
		_Color ("Outline Color", Color) = (1,0,1,1)
		_Width("Outline Width", Float) = 20
	}



	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		//Horizontal Pass
		Pass{

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex, _Scene, _Mask;
			float4 _Color, _MainTex_TexelSize, _MainTex_ST;
			float _Width;

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


			float4 frag(v2f i) : SV_TARGET {

				float4 col = _Color;//* _ColorIntensity;
				float4 mask = tex2D(_Mask, i.uv.xy);
				float4 tex = tex2D(_MainTex, i.uv.xy);
				float4 scene = tex2D(_Scene, i.uv.xy);



				if( mask.r < 0.1){


	

					float4 mod = scene*2*tex.r*(1-col);


					return (1-mod*_Width) * scene;

					//float c = col.r;
					//return _Color * scene;

					//return (scene * (tex.r * _ColorIntensity));

					//return step(scene,tex);

					//if(tex.r > _ColorIntensity/1000){

					//	return col;

					//}

					//return scene;

					//return scene*tex.r;


					//return (scene + (tex.r * _ColorIntensity)*2*float4(0,0,1,1));

					//return tex.r * col * 2 + (1 - tex.r) * scene;

					//return (tex.r*scene+tex*col*2);

					//return scene*0.25 + tex*col*2;

					//return scene;

					//float3 col2 =  scene * (1 - tex.r);
					//float3 col3 = col2*0.25;
					//float3 ler = lerp(col2, col3, tex.a);

					//return float4(col3,1) + (tex*col*2);

					//if(tex.r > 0){
					//	return float4(col3,1) + (tex*2*col);
					//}

					//return scene+tex*col;
					//if(tex.r > 0){
						//return tex*col * scene*(1-tex.a);
					//}

					//return lerp(mask, tex, tex.a*20) * scene;

					//float3 r = lerp(scene.rgb, tex.rgb*col.rgb, tex.a);
					//return float4(r,1) + scene;
					//scene.rgb = max();//tex.rgb * 0.5 * col;
					//return scene;
					//return scene + tex2D(_Scene, i.uv.xy);
					//return saturate(scene + (tex * col));	
					//return tex2D(_Scene, i.uv.xy) + (_Color * tex2D(_MainTex, i.uv.xy)*_ColorIntensity);

				//	return (1-tex.a) * scene + (2*tex);
				}

				//return 0;
				return scene;
				//return tex2D(_Scene, i.uv.xy);
				
			}

		ENDCG
		}
	}
}
