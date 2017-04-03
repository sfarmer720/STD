//Code duplication gaurd
#if !defined(OUTPASS)
#define OUTPASS

	//Include files
	#include "UnityCG.cginc"

	//import properties
	float _OutlineSize;
	float4 _Outline;


	//Vertex Data Structure
	struct VertData {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	//Vertex Structure
	struct v2f {
		float4 pos : SV_POSITION;
		float3 normal : TEXCOORD0;
		float3 worldPos : TEXCOORD1;
	};


	//Vertex function
	v2f vert ( VertData v ){

		//Initialize vert structure
		v2f i;
		UNITY_INITIALIZE_OUTPUT(v2f, i);

		//Check if actively rendering outline
		#if defined(_RENDER_OUTLINE)

			//set vertex positions
			i.pos = UnityObjectToClipPos( v.vertex );

			//Set normals

			float3 norm = normalize( mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));

			//set offset
			float2 offset = TransformViewToProjection(norm.xy);

			//offset the normals from initial position
			i.pos.xy += offset * i.pos.z * _OutlineSize;


			//Set expanded normals and offset amount
			//float3 outNorm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
			//float2 outOff = TransformViewToProjection(outNorm.xy);

			//modify vertex position 
			//i.pos.xy += outOff * i.pos.z * _OutlineSize; 
			//v.vertex.xyz += v.normal.xyz * _OutlineSize;
			//i.pos = mul( UNITY_MATRIX_MVP, v.vertex );



		#endif

		return i;
	}

	//Fragment function
	float4 frag() : SV_TARGET {

		//Check if actively rendering outline
		#if defined(_RENDER_OUTLINE)
			return _Outline;
		#else
			return 0;
		#endif
	}

#endif