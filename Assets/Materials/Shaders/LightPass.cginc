//Code duplication gaurd
#if !defined(LIGHTPASS)
#define LIGHTPASS

			//Include Files
			#include "UnityPBSLighting.cginc"
			#include "AutoLight.cginc"

			//Property Variables
			float _NormalScale, _BumpScale, _Metal, _Smooth, _OccIntensity, _AlphaCut;
			float4 _Tint, _MainTex_ST, _Bump_ST;
			sampler2D _MainTex, _Normal, _Bump, _BumpNormal, _BumpMask, _MetalMap, _EmissionMap, _OccMap;
			float3 _Emission;


			//Vertex Data Structure
			struct vData {

				//Position, UV, Normals, and tangent
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;

			};

			//Vertex Structure
			struct v2f {

				//Position, UV, Normal
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;

				//Define Tangent based on binormal calculation in vert or frag function
				#if defined (BINORMAL_PER_FRAGMENT)
					float4 tangent : TEXCOORD2;
				#else
					float3 tangent : TEXCOORD2;
					float3 binormal : TEXCOORD3;
				#endif

				//World Position
				float3 worldPos : TEXCOORD4;

				//Set shadow coordinates
				SHADOW_COORDS( 5 )

				//Check if calculating light in vertex function
				#if defined(VERTEXLIGHT_ON)

					// define vertex color variable
					float3 vertexLightColor : TEXCOORD6;

				#endif

			};

			//Apply bump mask
			float GetMask ( v2f i ){
				#if defined (_BUMP_MASK)
					return tex2D(_BumpMask, i.uv.xy).a;
				#else
					return 1;
				#endif
			}

			//Set Albedo Color
			float3 GetAlbedo ( v2f i ){

				//Set initial albedo
				float3 albedo = tex2D ( _MainTex, i.uv.xy ).rgb * _Tint.rgb;

				//Check if using Bummp Map
				#if defined(_BUMP_MAP)

					//set details
					float3 bump = tex2D ( _Bump, i.uv.zw ) * unity_ColorSpaceDouble;

					//Lerp details and albedo
					albedo = lerp( albedo, albedo * bump, GetMask( i ));

				#endif
				return albedo;

			}

			//Set alpha for transparency
			float GetAlpha ( v2f i ) { 

				//Initialize alpha
				float alpha = _Tint.a;

				//Check if using alpha for smoothness
				#if !defined(_SMOOTHNESS_ALBEDO)

					//set alpha from texture
					alpha *= tex2D( _MainTex, i.uv.xy ).a;

				#endif
				return alpha;
			}

			//Metal / Dielectric based on map or value
			float GetMetal ( v2f i ){

				//Check for metal map
				#if defined(_METAL_MAP)

					//defined map, sample and return
					return tex2D( _MetalMap, i.uv.xy).r;

				#else

					//undefined map, return metal only
					return _Metal;

				#endif
			}

			//Smooth based on metal map or value
			float GetSmooth ( v2f i ){

				//initialize smooth
				float smooth = 1;

				//Check if albedo map smooth defined
				#if defined(_SMOOTHNESS_ALBEDO)
					smooth = tex2D( _MainTex, i.uv.xy ).a;

				//Check if Metal map smooth defined
				#elif defined(_SMOOTHNESS_ALBEDO) && defined (_METAL_MAP)
					smooth = tex2D( _MetalMap, i.uv.xy ).a;

				#endif

				//return input by float
				return smooth * _Smooth;
			}

			//Emission light based on maps
			float3 GetEmission ( v2f i ){

				//apply only to main light pass if emission map set
				#if defined(FORWARD_BASE_PASS)
					#if defined(_EMISSION_MAP)
						return tex2D (_EmissionMap, i.uv.xy) * _Emission;
					#else
						return _Emission;
					#endif
				#else
					return 0;
				#endif
			}

			//Occlusion Map Sampling
			float GetOcclusion ( v2f i ){

			//Check if map is defined
			#if defined(_OCC_MAP)
				return lerp( 1, tex2D( _OccMap, i.uv.xy ).g, _OccIntensity);
			#else
				return 1;
			#endif

			}

			//Calculate light in vertex function
			void CalcVertLight ( inout v2f i ){

				//Confirm vertex light calc is enabled
				#if defined(VERTEXLIGHT_ON)

				//set vertex light color
				i.vertexLightColor = Shade4PointLights (
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[4].rgb,
					unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, i.worldPos, i.normal
				); 

				#endif
			}

			//Calcuate the binormal
			float3 CalcBinormal ( float3 normal, float3 tangent, float biSign ){
				return cross( normal, tangent.xyz ) * ( biSign * unity_WorldTransformParams.w );
			}


			//Vertex Function
			v2f vert( vData v ){

				//Initialize vert structure
				v2f i;
				UNITY_INITIALIZE_OUTPUT(v2f, i);

				//set vertex positions
				i.pos = UnityObjectToClipPos( v.vertex );

				//Set World position
				i.worldPos = mul( unity_ObjectToWorld, v.vertex );

				//Set Normals
				i.normal = UnityObjectToWorldNormal(v.normal);

				//Check if binormal calc in vert or fragment
				#if defined(BINORMAL_PER_FRAGMENT)

					//Calc in fragment, set only tangent 
					i.tangent = float4 ( UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w );
				#else

					//Calc binormal and set tangent
					i.tangent = UnityObjectToWorldDir ( v.tangent.xyz );
					i.binormal = CalcBinormal ( i.normal, i.tangent, v.tangent.w);

				#endif

				//Set UVs
				i.uv.xy = TRANSFORM_TEX( v.uv, _MainTex );
				i.uv.zw = TRANSFORM_TEX( v.uv, _Bump );

				//Calculate shadows
				TRANSFER_SHADOW ( i );

				//claculate vertex lights
				CalcVertLight(i);

				return i;
			}

			//Initialize tangent normals
			float3 TangentNormals ( v2f i ){

				//Initialize return normal
				float3 norm = float3( 0, 0, 1);

				//Check if normal map is defined
				#if defined(_NORMAL_MAP)

					//Unpack & scale Normal Map
					norm = UnpackScaleNormal( tex2D( _Normal, i.uv.xy ), _NormalScale);

				#endif

					//Check if detail normal map defined
					#if defined(_BUMP_MAP_NORMAL)

					//Unpack & scale detail map
					float3 bumpNorm = UnpackScaleNormal( tex2D( _BumpNormal, i.uv.xy), _BumpScale);
					bumpNorm = lerp( float3( 0, 0, 1), bumpNorm, GetMask( i ));

					//Blend regular and detail normals
					norm = BlendNormals ( norm, bumpNorm );

				#endif

				return norm;
			}

			//Normalize fragment normals
			void FragNorm ( inout v2f i ){

				//Set tangent normals
				float3 tangentNormal = TangentNormals( i );

				//Check if calc binormal in vert or fragment
				#if defined(BINORMAL_PER_FRAGMENT)

					//Calculate binormal
					float3 binormal = CreateBinormal( i.normal, i.tangent.xyz, i.tangent.w);

				#else

					//Binormal already calculated
					float binormal = i.binormal;

				#endif

				//Convert from tangent to world space
				i.normal = normalize (
					tangentNormal.x * i.tangent +
					tangentNormal.y * binormal + 
					tangentNormal.z * i.normal
				);
			}

			//Box Projection support for indirect light
			float3 BoxProj (float3 direction, float3 position, float4 cubePos, float3 boxMin, float3 boxMax ){

			//Check if using Box Projection, Force branch, platform secure
			#if UNITY_SPECCUBE_BOX_PROJECTION
			UNITY_BRANCH
				if( cubePos.w > 0 ){

					//calculate direction scales
					float3 f = (( direction > 0 ? boxMax : boxMin ) - position ) / direction;

					//set smallest scale
					float s = min( min( f.x, f.y ), f.z );

					//set reflect direction
					direction =  direction * s + ( position - cubePos );
				}
			#endif
				//return 
				return direction;
			} 

			//Create Main Lights
			UnityLight CreateLight ( v2f i ){

				//Initialize light
				UnityLight light;

				//Check if directional light
				#if defined(POINT) || defined(POINT_COOKIE) || defined (SPOT)

					//Set normalized light direction
					light.dir = normalize( _WorldSpaceLightPos0.xyz - i.worldPos );
				#else

					//Set directional light varient
					light.dir = _WorldSpaceLightPos0.xyz;
				#endif

				//Set light attenuation
				UNITY_LIGHT_ATTENUATION( attenuation, i, i.worldPos);

				//Set Light color
				light.color = _LightColor0.rgb * attenuation;

				//Set nDot
				light.ndotl = DotClamped ( i.normal, light.dir );

				//return light
				return light;
			}

			//Create Indirect lights for vertex colors
			UnityIndirect CreateIndirectLight ( v2f i, float3 viewDir ){

				//Initialize dead light
				UnityIndirect inLight;
				inLight.diffuse = 0;
				inLight.specular = 0;

				//check if using vertex light calc
				#if defined(VERTEXLIGHT_ON)

					//Set indirect light color to vertex calc color
					inLight.diffuse = i.vertexLightColor;

				#endif

				//check if original light pass
				#if defined(FORWARD_BASE_PASS)

					//add positive sphere harmonic ambient light to indirect diffuse
					inLight.diffuse += max ( 0, ShadeSH9( float4( i.normal, 1)));

					//set specular reflection direction
					float3 reflectDir = reflect( -viewDir, i.normal );

					//set environment sample rough values
					Unity_GlossyEnvironmentData enviroData;
					enviroData.roughness = 1 - GetSmooth( i );

					//Set reflection based on box projection of environment
					enviroData.reflUVW = BoxProj( 
						reflectDir, i.worldPos,
						unity_SpecCube0_ProbePosition,
						unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax
					);

					//set initial reflection
					float3 probe0 = Unity_GlossyEnvironment(
						UNITY_PASS_TEXCUBE( unity_SpecCube0 ), unity_SpecCube0_HDR, enviroData
					);

					//Set reflection based on box projection of second probe
					enviroData.reflUVW = BoxProj( 
						reflectDir, i.worldPos,
						unity_SpecCube1_ProbePosition,
						unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax
					);

					//define reflection interpolation
					float refLerp = unity_SpecCube0_BoxMin.w;

					//Check if second probe needed, force branch, platform secure
					#if UNITY_SPECCUBE_BLENDING
					UNITY_BRANCH
					if( refLerp < 0.999 ){

						//set second reflection
						float3 probe1 = Unity_GlossyEnvironment(
							UNITY_PASS_TEXCUBE_SAMPLER( unity_SpecCube1, unity_SpecCube0 ),
							unity_SpecCube0_HDR, enviroData
						);

						//define specular color from HDR decoded environment sample
						inLight.specular = lerp( probe1, probe0, refLerp );

					}else{

						//define specular on environment alone
						inLight.specular = probe0;
					}
					#else

						//define specular on environment alone for unsupported platform
						inLight.specular = probe0;

					#endif

					//set indirect light occlusion
					float occ = GetOcclusion( i );
					inLight.diffuse *= occ;
					inLight.specular *= occ;

				#endif

				//return indirect light
				return inLight;
			}

			//Fragment function
			float4 frag( v2f i ) : SV_TARGET{

				//Set alpha 
				float alpha = GetAlpha( i );

				//Check if using cutout rendering
				#if defined(_RENDER_CUT)

					//if using cutout rendering, discard fragments below threshold
					clip( alpha - _AlphaCut );
				#endif

				//Normalize fragment and set direction
				i.normal = normalize ( i.normal );
				float3 viewDir = normalize ( _WorldSpaceCameraPos - i.worldPos );

				//Set Color & Texture


				//PBR mod albedo
				float3 specTint;
				float oneMinusReflectivity;
				float3 albedo = DiffuseAndSpecularFromMetallic( 
					GetAlbedo( i ), GetMetal( i ), specTint, oneMinusReflectivity
				);

				//Transparent alpha application
				#if defined(_RENDER_TRANS)
					albedo *= alpha;
					alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
				#endif

				//set fragment color
				float4 col =  UNITY_BRDF_PBS (
												albedo, 
												specTint,
												oneMinusReflectivity,
												GetSmooth( i ),
												i.normal,
												viewDir,
												CreateLight( i ),
												CreateIndirectLight( i, viewDir )
											);

				//apply emission to fragment
				col.rgb += GetEmission( i );

				//Check if transparent renderering
				#if defined(_RENDER_FADE) || defined(_RENDER_TRANS)

					//Set colors alpha
					col.a = alpha;

				#endif

				return col;
			}   

#endif