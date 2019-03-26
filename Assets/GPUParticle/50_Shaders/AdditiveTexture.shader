Shader "GPUParticle/AdditiveTexture" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha One
			AlphaTest Greater .01
			ColorMask RGB
			Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
				Bind "TexCoord", texcoord
			}

			// ---- Fragment program cards
			SubShader {
				Pass {

					CGPROGRAM
					#pragma vertex vert
					#pragma geometry geom
					#pragma fragment frag
					#pragma fragmentoption ARB_precision_hint_fastest
					#pragma multi_compile_particles

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					fixed4 _TintColor;

					struct v2f {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						#ifdef SOFTPARTICLES_ON
						float4 projPos : TEXCOORD1;
						#endif
						float scale : TEXCOORD2;
					};

					float4 _MainTex_ST;

#include "Assets/GpuParticle/51_ComputeShaders/ParticleData.cginc"

					StructuredBuffer<ParticleData> _Particles;

					v2f vert(uint id: SV_VertexID)
					{
						v2f o;
						/*o.vertex = UnityObjectToClipPos(v.vertex);
						#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos(o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
						#endif
						o.color = v.color;
						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);*/

						o.vertex = float4(_Particles[id].position, 1);
						o.texcoord = float2(0, 0);
						o.color = _Particles[id].color;
						o.scale = _Particles[id].isDead == 1 ? 0 : _Particles[id].scale;
						return o;
					}

					[maxvertexcount(4)]
					void geom(point v2f input[1], inout TriangleStream<v2f> outStream)
					{
						v2f output;
						UNITY_INITIALIZE_OUTPUT(v2f, output);


						// 全ての頂点で共通の値を計算しておく
						float4 pos = input[0].vertex;
						float4 col = input[0].color;

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
								output.texcoord = tex;

								// 頂点位置を計算
								output.vertex = pos + mul(float4((tex * 2 - float2(1, 1)) * input[0].scale, 0, 1), billboardMatrix);
								output.vertex = mul(UNITY_MATRIX_VP, output.vertex);

								// 色
								output.color = col;

								// ストリームに頂点を追加
								outStream.Append(output);
							}
						}

						// トライアングルストリップを終了
						outStream.RestartStrip();
					}

					sampler2D _CameraDepthTexture;
					float _InvFade;

					fixed4 frag(v2f i) : COLOR
					{
						#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
						float partZ = i.projPos.z;
						float fade = saturate(_InvFade * (sceneZ - partZ));
						i.color.a *= fade;
						#endif

						return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
					}
					ENDCG
				}
			}

			// ---- Dual texture cards
			SubShader {
				Pass {
					SetTexture[_MainTex] {
						constantColor[_TintColor]
						combine constant * primary
					}
					SetTexture[_MainTex] {
						combine texture * previous DOUBLE
					}
				}
			}

			// ---- Single texture cards (does not do color tint)
			SubShader {
				Pass {
					SetTexture[_MainTex] {
						combine texture * primary
					}
				}
			}
		}
}