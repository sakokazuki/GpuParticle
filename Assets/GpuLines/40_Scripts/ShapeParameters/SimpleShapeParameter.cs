using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	
	public class SimpleShapeParameter : ShapeParameter {

		#region Public methods
		public override void ShapeUpdate(ComputeShader computeShader)
		{
			if (isUpdate == false) return;
			computeShader.SetFloat("_LineAngle", lineAngle);
			computeShader.SetFloat("_VelocityScale", velocityScale);
		} 
		#endregion

	}
		
}


