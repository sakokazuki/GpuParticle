using UnityEngine;
using System.Runtime.InteropServices;

namespace SKZ.GPULines {
	
	public class LineShape : MonoBehaviour {
		#region Public variables

		public int LineCount
		{
			get { return lineCount; }
		}

		public int SegmentCount
		{
			get { return segmentCount; }
		}

		public RenderTexture PositionRT
		{
			get { return positionRT; }
		}

		public RenderTexture NormalRT
		{
			get { return normalRT; }
		}

		public RenderTexture VelocityRT
		{
			get { return velocityRT; }
		}

		public bool IsReady
		{
			get { return frameCount > 1; }
		}
		#endregion

		#region Private variables
		[SerializeField]
		ComputeShader computeShader;
		[SerializeField]
		int lineCount;
		[SerializeField]
		int segmentCount;
		[SerializeField]
		ShapeParameter parameter;
		[SerializeField]
		Camera eyeCmaera;

		int kernelIndex;
		Vector3 groupSize;
		int frameCount = 0;

		RenderTexture positionRT;
		RenderTexture normalRT;
		RenderTexture velocityRT;

		const string positionBufferName = "positionBuffer";
		const string normalBufferName = "normalBuffer";
		const string velocityBufferName = "velocityBuffer";
		#endregion

		#region Private methods

		RenderTexture CreateRenderTexture()
		{
			var rt = new RenderTexture(lineCount, segmentCount, 0, RenderTextureFormat.ARGBFloat);
			rt.enableRandomWrite = true;
			rt.Create();
			return rt;
		}

		void NewRT()
		{
			if (positionRT != null) return;
			positionRT = CreateRenderTexture();
			normalRT = CreateRenderTexture();
			velocityRT = CreateRenderTexture();
		}

		void ReleaseRT()
		{
			if (positionRT == null) return;
			positionRT.Release();
			normalRT.Release();
			velocityRT.Release();
		}
		void InitKernel()
		{
			NewRT();

			//set kernel index
			kernelIndex = computeShader.FindKernel("CSMain");
			
			//thread size
			uint threadSizeX, threadSizeY, threadSizeZ;
			computeShader.GetKernelThreadGroupSizes(kernelIndex,
				out threadSizeX, out threadSizeY, out threadSizeZ);
			int groupSizeX = Mathf.CeilToInt(lineCount / (float)threadSizeX);
			int groupSizeY = Mathf.CeilToInt(segmentCount / (float)threadSizeY);
			int groupSizeZ = (int)threadSizeZ;
			groupSize = new Vector3(groupSizeX, groupSizeY, groupSizeZ);

			computeShader.SetTexture(kernelIndex, positionBufferName, positionRT);
			computeShader.SetTexture(kernelIndex, normalBufferName, normalRT);
			computeShader.SetTexture(kernelIndex, velocityBufferName, velocityRT);

		}
		void DispachKernel()
		{
			//カメラに正対なnormalを指定するため。指定のない場合は(0, -1, 0)を基準にする。
			var worldCameraPos = eyeCmaera == null ? 
				Vector3.back*1000 : eyeCmaera.transform.position;
			
			computeShader.SetVector("_WorldCameraPosition", worldCameraPos);

			//Debug.Log(transform.localToWorldMatrix);
			//computeShader.SetMatrix("_WorldMatrix", transform.localToWorldMatrix);
			parameter.ShapeUpdate(computeShader);
			computeShader.Dispatch(kernelIndex, 
				(int)groupSize.x, (int)groupSize.y, (int)groupSize.z);

		}



		#endregion

		#region MonoBehaviour functions
		
		private void OnDestroy()
		{
			ReleaseRT();
		}

		// Use this for initialization
		void Start()
		{
			computeShader = Instantiate(computeShader);
			InitKernel();
			
		
		}

		void LateUpdate()
		{
			if(frameCount < 100) //100 is no means.
				frameCount++;

			DispachKernel();
		}

		#endregion
	}

}


