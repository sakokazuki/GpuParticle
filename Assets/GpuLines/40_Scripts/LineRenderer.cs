using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	[RequireComponent(typeof(LineShape))]
	public class LineRenderer : MonoBehaviour {

		#region Private variables
		LineShape shape;
		LineMeshes meshes;

		[SerializeField]
		RendererParameter parameter;
		[SerializeField]
		GameObject rendererPrefab;

		RendererAdapter[] renderers;
		MaterialPropertyBlock propertyBlock;

		private Material material;
		#endregion

		#region Public methods

		#endregion

		#region Private methods
		void UpdateRenderer()
		{
			if(renderers == null)
			{
				var meshes = new LineMeshes(shape.LineCount, shape.SegmentCount);
				renderers = new RendererAdapter[meshes.RenderMeshes.Length];
				for(int i=0; i<renderers.Length; i++)
				{
					var go = Instantiate(rendererPrefab, transform);
					go.transform.localPosition = Vector3.zero;
					go.transform.localRotation = Quaternion.identity;
					var renderer = go.GetComponent<RendererAdapter>();
					renderer.Setup(
						material, propertyBlock, meshes.RenderMeshes[i]);

					renderers[i] = renderer;
				}
			}

			var block = propertyBlock;
			block.SetTexture("_PositionBuffer", shape.PositionRT);
			block.SetTexture("_VelocityBuffer", shape.VelocityRT);
			block.SetTexture("_OrthnormBuffer", shape.NormalRT);
			parameter.RenderUpdate(propertyBlock);
		}
		#endregion

		#region MonoBehaviour functions

		// Use this for initialization
		void Awake ()
		{
			shape = GetComponent<LineShape>();
			propertyBlock = new MaterialPropertyBlock();
			material = new Material(parameter.Shader);


		}
	
		// Update is called once per frame
		void LateUpdate()
		{
			UpdateRenderer();
		}
		#endregion
	}
		
}


