using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SKZ.GPULines {
	
	public class TestKernel : MonoBehaviour {

		#region Private variables
		[SerializeField]
		ComputeShader computeShader;

		//test
		int intKernalIndex;
		ComputeBuffer intBuffer;

		[System.Serializable]
		//変数の定義順をシェーダーと合わせる必要あり
		//(変数の値がずれる)
		struct TestData
		{
			public int Id;
			public Vector3 Position;
			public Vector3 Velocity;
		}
		int testDataKernelIndex;
		ComputeBuffer testDataBuffer;

		#endregion

		#region Private methods
		void InitIntKernel()
		{
			intKernalIndex = computeShader.FindKernel("IntCS");

			intBuffer = new ComputeBuffer(4, sizeof(int));

			//set default data
			int[] initData = new int[4];
			for (var i = 0; i < 4; i++)
			{
				initData[i] = 1;
			}
			intBuffer.SetData(initData);
			initData = null;

			computeShader.SetBuffer(intKernalIndex, "intBuffer", intBuffer);

			computeShader.SetInt("intValue", 1);
		}
		void DispachIntKernel()
		{
			computeShader.Dispatch(intKernalIndex, 1, 1, 1);

			int[] result = new int[4];

			this.intBuffer.GetData(result);

			Debug.Log("RESULT : IntCS");

			for (var i = 0; i < 4; i++)
			{
				Debug.Log(result[i]);
			}
			result = null;
		}

		void InitTestDataKernel()
		{
			testDataKernelIndex = computeShader.FindKernel("TestDataCS");
			testDataBuffer = new ComputeBuffer(4, 
				Marshal.SizeOf(typeof(TestData)));

			//set default data
			TestData[] initData = new TestData[4];
			for (var i = 0; i < 4; i++)
			{
				var initPos = new Vector3(0, 1, 0);
				initData[i] = new TestData {
					Id =i,
					Position = initPos,
					Velocity = initPos,
				};
			}
			testDataBuffer.SetData(initData);
			initData = null;

			computeShader.SetBuffer(testDataKernelIndex, "testDataBuffer", testDataBuffer);
		}

		void DispachTestDataKernel()
		{
			computeShader.Dispatch(testDataKernelIndex, 1, 1, 1);

			TestData[] result = new TestData[4];

			this.testDataBuffer.GetData(result);

			Debug.Log("RESULT : TestDataCS");

			for (var i = 0; i < 4; i++)
			{
				Debug.LogFormat("id: {0} position: {1} velocity: {2}",
					result[i].Id, result[i].Position, result[i].Velocity);
			}
			result = null;
		}

		#endregion

		#region MonoBehaviour functions

		// Use this for initialization
		void Start() {
			//setup kenrel
			InitIntKernel();

			//test twice dispach
			for (var i=0; i<2; i++)
			{
				DispachIntKernel();
			}

			InitTestDataKernel();
			//test 3 frame dispatch
			for (var i=0; i<3; i++)
			{
				
				DispachTestDataKernel();
			}
		
		}
	
		#endregion
	}


		
}


