using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPUParticle {
	
	public class MeshParticleRenderer : MonoBehaviour {

		#region Private variables
		[SerializeField]
		GPUParticleEmitter emitter;

		[SerializeField]
		Material material;
		[SerializeField]
		Mesh mesh;


		//0: index  count per 1 instance	1: instance count
		//2: start index position			3: base vertex position
		//4: start instance position
		uint[] args = new uint[5]{ 0, 0, 0, 0, 0 };
		ComputeBuffer argsBuffer;
		
		#endregion

		#region Public methods

		#endregion

		#region Private methods
		void RenderInstancedMesh()
		{
			uint numIndecies = mesh == null ? 0 : (uint)mesh.GetIndexCount(0);

			args[0] = numIndecies;
			args[1] = (uint)emitter.ParticleNum;
			argsBuffer.SetData(args);

			material.SetBuffer("_Particles", emitter.ParticleBuffer);
			//適当
			var bounds = new Bounds(Vector3.zero, Vector3.one * 10);
			Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);

		}
		#endregion

		#region MonoBehaviour functions

		// Use this for initialization
		void Start (){
			argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		}
	
		// Update is called once per frame
		void Update () {
			RenderInstancedMesh();
		}

		private void OnDestroy()
		{
			if(argsBuffer != null)
			{
				argsBuffer.Release();
				argsBuffer = null;
			}
		}
		#endregion
	}
		
}


