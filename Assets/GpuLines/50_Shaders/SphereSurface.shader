Shader "Hidden/GPULines/SphereSurface" {
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

		float _LineHueRange;
		float _SegmentHueRange;
		half4 _BaseColor;

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
		#include "Assets/Cgincs/Noise/SimplexNoise2D.cginc"
		#include "Assets/Cgincs/Color.cginc"

		float LineWidth(float3 velocity) {
			float velStrength = length(velocity);
			return _LineWidth + velStrength;
		}

		half3 GradColor(fixed3 baseCol, fixed3 addCol, float addPoint, float range, float p) {
			fixed3 c = baseCol;
			float p1 = smoothstep(addPoint - range / 2.0, addPoint, p);
			float p2 = smoothstep(addPoint + range, addPoint, p);
			float control = p < addPoint ? p1 : p2;
			return lerp(baseCol, addCol, control);
		}

		half3 SurfaceColor(float lineId, float segmentPos) {
			float num = 3;
			segmentPos += floor(num*lineId) * 0.6;
			segmentPos = frac(segmentPos);

			float noise2d = snoise(float2(lineId * 3, _Time.y / 2.0));
			float gradMove = -_Time.y;

			float positionHueShift = _LineHueRange * sin(segmentPos*UNITY_PI * 2);
			float heightHueShift = _SegmentHueRange * lineId;


			float segmentPosEx = frac(segmentPos * 2 + _Time.x);
			float whiteRange = (1 + noise2d) / 2 * 0.5;
			float whiteRate = segmentPosEx < 0.5 ? smoothstep(0.5 - whiteRange, 0.5, segmentPosEx) : smoothstep(0.5 + whiteRange, 0.5, segmentPosEx);
			whiteRate *= 0.5;

			fixed3 base = ShiftColor(_BaseColor.xyz, fixed3(0, 1 - whiteRate, 1));
			fixed3 col = ShiftColor(base, float3(frac(_Time.x), 1, 1));
			col = ShiftColor(col, float3(positionHueShift, 1, 1));
			col = ShiftColor(col, float3(heightHueShift, 1, 1));
			//return col;

#if UNITY_COLORSPACE_GAMMA
			return col;
#else
			return GammaToLinearSpace(col);
#endif
		}

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


			float lw = LineWidth(V);
			half width = lw * io.vertex.z;

			// asign position, normal
			io.vertex = float4(P + binormal * width, io.vertex.w);
			io.normal = normal;

			//color;
			half3 col = SurfaceColor(texcoord.x, texcoord.y);
			//col = half3(0, 1, 0);
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
