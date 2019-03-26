using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SKZ.GPUParticle {
	

	public class SimpleEmitter : GPUParticleEmitter
	{
		#region Public variables
		public override ComputeBuffer ParticleBuffer
		{
			get { return particleBuffer; }
		}

		public override int ParticleNum
		{
			get { return capability; }
		}
		#endregion

		#region Private variables


		[SerializeField]
		ComputeShader computeShader;
		ComputeBuffer particleBuffer;
		ComputeBuffer particlePoolBuffer;
		ComputeBuffer argsBuffer;

		[SerializeField]
		int capability;
		[SerializeField]
		int emitNum;

		int[] particleArgs;

		const int THREAD_NUM_X = 8;
		const string INIT_KERNEL = "Init";
		const string EMIT_KERNEL = "Emit";
		const string UPDATE_KERNEL = "Update";
		int initializeKernelIndex;
		int emitKernelIndex;
		int updateKernelIndex;

		#endregion

		#region Public methods
		public override void Emit()
		{
			EmitKernel();
		}

		#endregion

		#region Private methods
		void InitializeKernel()
		{
			initializeKernelIndex = computeShader.FindKernel(INIT_KERNEL);
			emitKernelIndex = computeShader.FindKernel(EMIT_KERNEL);
			updateKernelIndex = computeShader.FindKernel(UPDATE_KERNEL);

			particleBuffer = new ComputeBuffer(capability, Marshal.SizeOf(typeof(ParticleData)));
			particlePoolBuffer = new ComputeBuffer(capability, Marshal.SizeOf(typeof(int)), ComputeBufferType.Append);
			particlePoolBuffer.SetCounterValue(0);

			argsBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
			particleArgs = new int[] { 0 };
			argsBuffer.SetData(particleArgs);

			computeShader.SetBuffer(initializeKernelIndex, "_Particles", particleBuffer);
			computeShader.SetBuffer(initializeKernelIndex, "_DeadList", particlePoolBuffer);
			computeShader.Dispatch(initializeKernelIndex, particleBuffer.count / THREAD_NUM_X, 1, 1);

		}

		void EmitKernel()
		{
			argsBuffer.SetData(particleArgs);
			ComputeBuffer.CopyCount(particlePoolBuffer, argsBuffer, 0);
			argsBuffer.GetData(particleArgs);

			int poolNum = particleArgs[0];
			if (poolNum < emitNum)
			{
				return;
			}

			computeShader.SetBuffer(emitKernelIndex, "_ParticlePool", particlePoolBuffer);
			computeShader.SetBuffer(emitKernelIndex, "_Particles", particleBuffer);

			computeShader.SetMatrix("_WorldMatrix", transform.localToWorldMatrix);
			computeShader.Dispatch(emitKernelIndex, emitNum / THREAD_NUM_X, 1, 1);

			
			//ParticleData[] datas = new ParticleData[particleBuffer.count];
			//particleBuffer.GetData(datas);
			//Debug.Log("=======");
			//for (var i = 0; i < datas.Length; i++)
			//{
			//	var p = datas[i];
			//	Debug.LogFormat("data[{0}]\nisDead: {1},\n position: {2},\n scale: {3},\n velocity: {4},\n duration: {5},\n color: {6}",
			//		i, p.IsDead, p.Position, p.Scale, p.Velocity, p.Duration, p.Color);

			//}
		}

		void UpdateKernel()
		{
			computeShader.SetBuffer(updateKernelIndex, "_Particles", particleBuffer);
			computeShader.SetBuffer(updateKernelIndex, "_DeadList", particlePoolBuffer);
			computeShader.SetFloat("_DeltaTime", Time.deltaTime);
			computeShader.Dispatch(updateKernelIndex, particleBuffer.count / THREAD_NUM_X, 1, 1);
		}

		void ReleaseKernel()
		{
			if (particleBuffer != null)
			{
				particleBuffer.Release();
			}

			if (particlePoolBuffer != null)
			{
				particlePoolBuffer.Release();
			}

			if (argsBuffer != null)
			{
				argsBuffer.Release();
			}
		}
		#endregion

		#region MonoBehaviour functions

		// Use this for initialization
		void Start ()
		{
			InitializeKernel();
		}

		// Update is called once per frame
		void Update ()
		{
			if (Input.GetMouseButton(0))// if once GetMouseButtonDown
			{
				EmitKernel();
			}
			UpdateKernel();
		}

		private void OnValidate()
		{
			capability = Mathf.CeilToInt((float)capability / THREAD_NUM_X) * THREAD_NUM_X;
			emitNum = Mathf.CeilToInt((float)emitNum / THREAD_NUM_X) * THREAD_NUM_X;
		}

		private void OnDestroy()
		{
			ReleaseKernel();
		}
		#endregion
	}
		
}


