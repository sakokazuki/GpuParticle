Shader "GPUParticle/InstancedMeshBillboardSurface" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", Range(0, 10)) = 1.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			Cull Off
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert addshadow
			#pragma instancing_options procedural:setup

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

	#include "Assets/GpuParticle/51_ComputeShaders/ParticleData.cginc"

			half _Glossiness;
			half _Metallic;
			half _Emission;
			fixed4 _Color;

	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			StructuredBuffer<ParticleData> _Particles;
	#endif



			float4x4 rotateAxisX(float a) {
				return float4x4(1, 0, 0, 0,
					0, cos(a), -sin(a), 0,
					0, sin(a), cos(a), 0,
					0, 0, 0, 1);
			}

			float4x4 rotateAxisY(float a) {
				return float4x4(cos(a), 0, sin(a), 0,
					0, 1, 0, 0,
					-sin(a), 0, cos(a), 0,
					0, 0, 0, 1);

			}

			float4x4 rotateAxisZ(float a) {
				return float4x4(
					cos(a), -sin(a), 0, 0,
					sin(a), cos(a), 0, 0,
					0, 0, 1, 0,
					0, 0, 0, 1
					);
			}


			void vert(inout appdata_full v) {
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				ParticleData p = _Particles[unity_InstanceID];

				float3 pos = p.position;
				float3 scale = p.isDead == 1 ? 0 : p.scale;

				float4x4 object2world = (float4x4)0;
				object2world._11_22_33_44 = float4(scale, 1.0);

				float3 camdir = ObjSpaceViewDir(float4(pos, 0));
				//camdir *= -1;
				float rotY = atan2(-camdir.x, -camdir.z);
				float rotX = atan2(camdir.y, sqrt(camdir.x*camdir.x + camdir.z*camdir.z));

				float4x4 ry = rotateAxisY(rotY);
				float4x4 rx = rotateAxisX(rotX - UNITY_PI / 2);
				if (camdir.x != 0 || camdir.z != 0) {
					object2world = mul(rx, object2world);
					object2world = mul(ry, object2world);
				}
				object2world._m03 =
					object2world._m13 =
					object2world._m23 =
					object2world._m33 = 0;


				object2world._14_24_34 += pos;

				v.vertex = mul(object2world, v.vertex);
				v.normal = mul(object2world, v.normal);
				v.normal = normalize(v.normal);
	#endif
			}

			void setup() {}

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				if (length(c.rgb) < 0.2) {
					discard;
				}
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Emission = c * _Emission;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
