using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	
	public abstract class ShapeParameter : MonoBehaviour {


		#region Protected variables
		[SerializeField]
		protected bool isUpdate = true;
		[SerializeField, Range(-90, 90)]
		protected int lineAngle;
		[SerializeField]
		protected float velocityScale;
		#endregion

		#region Public methods
		abstract public void ShapeUpdate(ComputeShader computeShade);
		#endregion


	}
		
}


