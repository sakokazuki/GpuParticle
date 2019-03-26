using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPUParticle {
	struct ParticleData
	{
		public int IsDead;
		public Vector3 Position;
		public Vector3 Velocity;
		public Color Color;
		public float Duration;
		public float Scale;
		public float DurationMax;
		public float StartScale;
		public Vector3 StartVelocity;
	}


	public abstract class GPUParticleEmitter : MonoBehaviour
	{
		#region abstract properties
		public abstract ComputeBuffer ParticleBuffer { get; }
		public abstract int ParticleNum { get; }
		#endregion

		#region abstract functions
		public abstract void Emit();
		#endregion

	}
		
}


