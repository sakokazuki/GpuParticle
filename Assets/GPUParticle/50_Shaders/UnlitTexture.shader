Shader "GPUParticle/UnlitTexture" {
	Properties{
		_MainTex("Particle Texture", 2D) = "white" {}
	}
	SubShader{
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;

#include "Assets/GpuParticle/51_ComputeShaders/ParticleData.cginc"

			StructuredBuffer<ParticleData> _Particles;


			struct VSOut {
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
				float4 col : COLOR;
				float scale : TEXCOORD1;
			};

			VSOut vert(uint id: SV_VertexID) {
				VSOut o;
					
				o.pos = float4(_Particles[id].position, 1);
				o.tex = float2(0, 0);
				o.col = _Particles[id].color;
				o.scale = _Particles[id].isDead == 1 ? 0 : _Particles[id].scale;
				return o;
			}

			[maxvertexcount(4)]
			void geom(point VSOut input[1], inout TriangleStream<VSOut> outStream)
			{
				VSOut output;
				UNITY_INITIALIZE_OUTPUT(VSOut, output);


				// 全ての頂点で共通の値を計算しておく
				float4 pos = input[0].pos;
				float4 col = input[0].col;

				// 四角形になるように頂点を生産
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						// ビルボード用の行列
						float4x4 billboardMatrix = UNITY_MATRIX_V;
						billboardMatrix._m03 =
							billboardMatrix._m13 =
							billboardMatrix._m23 =
							billboardMatrix._m33 = 0;

						// テクスチャ座標
						float2 tex = float2(x, y);
						output.tex = tex;

						// 頂点位置を計算
						output.pos = pos + mul(float4((tex * 2 - float2(1, 1)) * input[0].scale, 0, 1), billboardMatrix);
						output.pos = mul(UNITY_MATRIX_VP, output.pos);

						// 色
						output.col = col;

						// ストリームに頂点を追加
						outStream.Append(output);
					}
				}

				// トライアングルストリップを終了
				outStream.RestartStrip();
			}


			fixed4 frag(VSOut i) : COLOR{
				float4 col = tex2D(_MainTex, i.tex) * i.col;
				//col = fixed4(1, 0, 0, 1);
				return col;
			}

			ENDCG
		}
	}
	
}
