using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	
	public abstract class RendererParameter : MonoBehaviour {
		#region Public variables
		public Shader Shader
		{
			get { return shader; }
		}
		#endregion

		#region Private variables
		[SerializeField]
		Shader shader;
		#endregion

		#region Protected variables

		[SerializeField]
		protected bool isUpdate = true;

		[SerializeField]
		protected float lineWidth = 0.4f;

		[SerializeField]
		protected int randomSeed = 0;

		#endregion

		#region Public methods
		abstract public void RenderUpdate(MaterialPropertyBlock block);
		#endregion


	}
		
}


