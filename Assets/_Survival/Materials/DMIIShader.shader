Shader "DMIIShader" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		
		[HideInInspector] _Instancing_Temp ("Instancing Temp", Float) = 1
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			// #pragma instancing_options procedural:setup

			struct vertex {
				float4 pos	: POSITION;
				float2 uv	: TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct fragment {
				float4 pos	: SV_POSITION;
				float2 uv	: TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
	UNITY_INSTANCING_BUFFER_START(instancingProps)
		UNITY_DEFINE_INSTANCED_PROP(float, _Instancing_Temp)
	UNITY_INSTANCING_BUFFER_END(instancingProps)

	// 		
	// 		CBUFFER_START(MyData)
	// 		    float4 posDirBuffer[100];
	// 		    float timeBuffer[100];
	// 		CBUFFER_END
	// 		
	// #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	// 		void setup() {
	// 			float2 position = posDirBuffer[unity_InstanceID].xy;
	// 			float2 direction = posDirBuffer[unity_InstanceID].zw;
	// 			direction *= smoothstep(0, 10, timeBuffer[unity_InstanceID]);
	//
	// 			unity_ObjectToWorld = float4x4(
	// 				direction.x, -direction.y, 0, position.x,
	// 				direction.y, direction.x, 0, position.y,
	// 				0, 0, 1, 0,
	// 				0, 0, 0, 1
	// 				);
	// 		}
	// #endif

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fragment vert(vertex v) {
				UNITY_SETUP_INSTANCE_ID(v);
				float instTemp = UNITY_ACCESS_INSTANCED_PROP(instancingProps, _Instancing_Temp);

				fragment f;
				// UNITY_TRANSFER_INSTANCE_ID(v, f);
				f.pos = UnityObjectToClipPos(v.pos);
				f.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// f.uv = v.uv;
				
				return f;
			}

			float4 frag(fragment f) : SV_Target{
				// UNITY_SETUP_INSTANCE_ID(f);
				float4 c = tex2D(_MainTex, f.uv);
				return c;
			}
			ENDCG
		}
	}
}
