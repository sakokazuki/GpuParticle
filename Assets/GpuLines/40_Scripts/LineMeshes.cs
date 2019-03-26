using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SKZ.GPULines {

	
	public class LineMeshes : IDisposable
	{
		#region Public variables

		public Mesh[] RenderMeshes
		{
			get { return renderMeshes; }
		}
		#endregion

		#region Private variables
		Mesh[] renderMeshes;

		int lineCount;
		int segmentCount;
		#endregion

		#region Public methods

		//Constructor
		public LineMeshes(int lineCount, int segmentCount)
		{
			this.lineCount = lineCount;
			this.segmentCount = segmentCount;

			Create();
		}

		//Destructor
		public void Dispose()
		{
			Destory();
		}


		#endregion

		#region Private methods

		Mesh CreateRenderMesh(int meshLineCount, int meshSegmentCount, int startLineId)
		{
			var mesh = new Mesh();
			var verticies = new List<Vector3>();

			for (int x = 0; x < meshLineCount; x++)
			{
				float u = ((float)x + startLineId + 0.5f) / lineCount;
				for (int y = 0; y < meshSegmentCount / 2; y++)
				{
					float v = ((float)y + 0.5f) / segmentCount;
					verticies.Add(new Vector3(u, v, -0.5f));
					verticies.Add(new Vector3(u, v, +0.5f));
				}
			}
			mesh.vertices = verticies.ToArray();

			var indices = new List<int>();
			int vi = 0;

			for (int x = 0; x < meshLineCount; x++)
			{
				for (int y = 0; y < meshSegmentCount / 2 - 1; y++)
				{
					indices.Add(vi + 0);
					indices.Add(vi + 2);
					indices.Add(vi + 1);

					indices.Add(vi + 1);
					indices.Add(vi + 2);
					indices.Add(vi + 3);

					vi += 2;
				}
				vi += 2;
			}

			mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

			mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10);
			mesh.name = string.Format("mesh ({0})", startLineId);
			mesh.UploadMeshData(true);

			return mesh;
		}

		void CreateRenderMeshes()
		{
			int meshCount = (lineCount * segmentCount * 2) / 0xffff + 1;
			renderMeshes = new Mesh[meshCount];

			int lineCountPerMesh = (0xffff / (segmentCount * 2));
			for (int i = 0; i < meshCount; i++)
			{
				int lastIndex = meshCount - 1;
				int meshLineCount = lineCountPerMesh;
				int meshSegmentCount = segmentCount * 2;
				if (i == lastIndex)
				{
					meshLineCount = lineCount - lineCountPerMesh * i;
				}

				renderMeshes[i] = CreateRenderMesh(meshLineCount, meshSegmentCount, i * lineCountPerMesh);
			}

		}

		void Destory()
		{
			foreach (var mesh in renderMeshes)
			{
				mesh.Clear();
				UnityEngine.Object.Destroy(mesh);
			}
			renderMeshes = null;
		}

		void Create()
		{
			CreateRenderMeshes();
		}
		#endregion


	}

	

}


