using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
namespace SKZ.GPUParticle {

	public class GPUParticleRenderer : MonoBehaviour {


		#region Private variables
		[SerializeField]
		GPUParticleEmitter emitter;

		[SerializeField]
		Material material;

		#endregion

		#region Private methods

		#endregion

		#region MonoBehaviour functions

		// Use this for initialization
		void Start () {
		}

		private void OnRenderObject()
		{
			material.SetBuffer("_Particles", emitter.ParticleBuffer);

			material.SetPass(0);
			Graphics.DrawProcedural(MeshTopology.Points, emitter.ParticleNum);

		}
		// Update is called once per frame
		void Update () {
			
		}


		#endregion
	}
		
}


