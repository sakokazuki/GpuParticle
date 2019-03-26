using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	
	public class ParticleBridge : MonoBehaviour {

		#region Private variables
		[SerializeField]
		LineShape shape;
		[SerializeField]
		LineParticleEmitter emitter;
		#endregion
		
		#region MonoBehaviour functions

		// Use this for initialization
		void Start () {
			
		}
	
		// Update is called once per frame
		void Update () {
			if (shape.IsReady == false) return;
			var p = shape.PositionRT;
			var v = shape.VelocityRT;
			emitter.SetTexture(p, v);
			emitter.Emit();
		}
		#endregion
	}
		
}


