using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {

	public class SimpleRendererParameter : RendererParameter
	{

		#region Public methods

		public override void RenderUpdate(MaterialPropertyBlock block)
		{
			if (isUpdate == false) return; 
			block.SetFloat("_LineWidth", lineWidth);
			block.SetFloat("_RandomSeed", randomSeed);
		}
		#endregion

	}
}


