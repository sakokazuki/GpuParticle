using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SKZ.GPUParticle;


namespace SKZ.GPULines
{
	public abstract class LineParticleEmitter : GPUParticleEmitter
	{
		public abstract void SetTexture(Texture posTex, Texture velTex);
	}
	

}


