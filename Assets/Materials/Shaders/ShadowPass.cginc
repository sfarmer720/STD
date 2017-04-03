#if !defined(SHADOWPASS)
#define SHADOWPASS


	//Include files
	#include "UnityCG.cginc"

	//Property Variables
	float4 _Tint, _MainTex_ST;
	sampler2D _MainTex, _DitherMaskLOD;
	float _AlphaCut;

	//Definitions
	#if defined(_RENDER_FADE) || defined(_RENDER_TRANS)
		#if(_SEMITRANS_SHADOWS)
			#define SHADOWS_SEMITRANSPARENT 1
		#else
			#define _RENDER_CUT
		#endif
	#endif
	#if SHADOWS_SEMITRANSPARENT || defined(_RENDER_CUT)
		#if !defined(_SMOOTH_ALBEDO)
			#define SHADOWS_NEED_UV 1
		#endif
	#endif



	//Vertex Data Structure
		struct ShadowData {

			//Shadow position and normals
			float4 position : POSITION;
			float4 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		//Shadow Cube vertex structure
		struct Shadowv2f {

			//Position
			float4 position : SV_POSITION;

			//Check if using cutout
			#if SHADOWS_NEED_UV
				float2 uv : TEXCOORD0;
			#endif

			//Check if using shadow cube
			#if defined(SHADOWS_CUBE)
				float3 lightVec : TEXCOORD0;
			#endif
		};

		//Shadows vert to Fragment
		struct v2f {

			//Set shadow position
			#if SHADOWS_SEMITRANSPARENT
				UNITY_VPOS_TYPE vpos : VPOS;
			#else
				float4 positions : SV_POSITION;
			#endif

			//set shadow uvs
			#if SHADOWS_NEED_UV
				float2 uv : TEXCOORD0;
			#endif

			//set light vector for cube map
			#if defined(SHADOWS_CUBE)
				float3 lightVec : TEXCOORD1;
			#endif
		};

		//Set alpha for transparency
		float GetAlpha ( v2f i ) { 

				//Initialize alpha
				float alpha = _Tint.a;

				//Check if using alpha for smoothness
				#if SHADOWS_NEED_UV

					//set alpha from texture
					alpha *= tex2D( _MainTex, i.uv.xy ).a;

				#endif
				return alpha;
			}

		//Vertex function
		Shadowv2f shadowVert ( ShadowData v ){

			//Initialize data
			Shadowv2f i;
			UNITY_INITIALIZE_OUTPUT(Shadowv2f, i);

			//check if using Shadow Cube
			#if defined(SHADOWS_CUBE)

				//Set shadow cube position
				i.position = UnityObjectToClipPos ( v.position );

				//sest shadow cube light vector
				i.lightVec = mul ( unity_ObjectToWorld, v.position ).xyz - _LightPositionRange.xyz;

			#else

				//Set shadow positions and return
				i.position = UnityClipSpaceShadowCasterPos ( v.position.xyz, v.normal );
				i.position = UnityApplyLinearShadowBias(i.position);

			#endif

			//Check if Usinge cutout
			#if SHADOWS_NEED_UV
				i.uv = TRANSFORM_TEX( v.uv, _MainTex);
			#endif

			return i;
		}

		//Fragment Function
		float4 shadowFrag ( v2f i ) : SV_TARGET{

			//Check for transparency
			float alpha = GetAlpha( i );
			#if SHADOWS_NEED_UV
				clip(alpha - _AlphaCut);
			#endif

			//Shadow Dithering
			#if SHADOWS_SEMITRANSPARENT
				float dither = tex2D( _DitherMaskLOD, float3( i.vpos.xy * 0.25, alpha * 0.9375 ));
				clip(dither - 0.01);
			#endif

			//check if using Shadow Cube
			#if defined(SHADOWS_CUBE)

				//Set shadow cube depth and return
			 	float depth = length ( i.lightVec ) + unity_LightShadowBias.x;
			 	depth *= _LightPositionRange.w;
			 	return UnityEncodeCubeShadowDepth ( depth );

			#else
				return 0;
			#endif

		}

#endif