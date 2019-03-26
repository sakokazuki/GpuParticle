using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines
{

	public class SphereRendererParameter : RendererParameter
	{

		#region Private variables
		[SerializeField]
		float lineHueRange;
		[SerializeField]
		float segmentHueRange;

		[SerializeField]
		Color baseColor;

		#endregion
		#region Public methods

		public override void RenderUpdate(MaterialPropertyBlock block)
		{
			if (isUpdate == false) return;
			block.SetFloat("_LineWidth", lineWidth);
			block.SetFloat("_RandomSeed", randomSeed);
			block.SetColor("_BaseColor", baseColor);
			block.SetFloat("_LineHueRange", lineHueRange);
			block.SetFloat("_SegmentHueRange", segmentHueRange);
		}
		#endregion

	}
}


