using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class RendererAdapter : MonoBehaviour
	{
		#region Public variables

		#endregion

		#region Private variables
		MeshRenderer meshRenderer;
		MeshFilter filter;

		Shader shader;
		Material material;
		MaterialPropertyBlock block;
		Mesh mesh;
		#endregion

		#region Public methods
		public void Setup(Material material, MaterialPropertyBlock block, Mesh mesh)
		{
			this.block = block;
			this.mesh = mesh;
			this.material = material;
		}
	
		#endregion


		#region MonoBehaviour functions

		// Use this for initialization
		void Awake()
		{
			meshRenderer = GetComponent<MeshRenderer>();
			filter = GetComponent<MeshFilter>();
		}

		// Update is called once per frame
		void Update()
		{
			if (filter.mesh != mesh)
			{
				filter.mesh = mesh;
			}

			if (meshRenderer.sharedMaterial != material)
			{
				meshRenderer.sharedMaterial = material;
			}
			meshRenderer.SetPropertyBlock(block);
		}
		#endregion
	}

}


