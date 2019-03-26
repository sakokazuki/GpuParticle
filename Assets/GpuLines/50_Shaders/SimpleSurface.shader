Shader "Hidden/GPULines/SimpleSurface" {
	Properties {
		_Albedo("Albedo", Color) = (0.5, 0.5, 0.5)
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		_Metallic("Metallic", Range(0, 1)) = 0

		[HideInInspector] _PositionBuffer("", 2D) = ""{}
		[HideInInspector] _OrthnormBuffer("", 2D) = ""{}
		[HideInInspector] _VelocityBuffer("", 2D) = ""{}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert nolightmap addshadow
		#pragma target 3.0

		sampler2D _PositionBuffer;
		sampler2D _VelocityBuffer;
		sampler2D _OrthnormBuffer;
		float4 _PositionBuffer_TexelSize;
		float _LineWidth;

		// Base material properties
		half3 _Albedo;
		half _Smoothness;
		half _Metallic;



		struct Input {
			half3 color : COLOR;
			float3 worldNormal;
			INTERNAL_DATA
		};

		
		#include "Assets/Cgincs/Utils.cginc"

		void vert(inout appdata_full io)
		{
			// Fetch samples from the animation kernel.
			float4 texcoord = float4(io.vertex.xy, 0, 0);
			float3 P = tex2Dlod(_PositionBuffer, texcoord).xyz;
			float3 V = tex2Dlod(_VelocityBuffer, texcoord), xyz;
			float4 B = tex2Dlod(_OrthnormBuffer, texcoord);

			// Extract normal/binormal vector from the orthnormal sample.
			half3 normal = StereoInverseProjection(B.xy);
			half3 binormal = StereoInverseProjection(B.zw);


			float lw = _LineWidth;
			half width = lw * io.vertex.z;

			// asign position, normal
			io.vertex = float4(P + binormal * width, io.vertex.w);
			io.normal = normal;

			//color;
			fixed3 col = fixed3(1, 0, 0);
			io.color.rgb = col;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = IN.color;
			o.Smoothness = _Smoothness;
			o.Metallic = _Metallic;
			o.Emission = IN.color * 2;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
