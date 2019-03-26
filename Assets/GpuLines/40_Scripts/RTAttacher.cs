using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPULines {
	
	public class RTAttacher : MonoBehaviour {


		#region Private variables
		[SerializeField]
		LineShape shape;
		
		enum Mode
		{
			Position,
			Normal,
			Velocity,
		}
		[SerializeField]
		Mode mode;

		Material material;
		#endregion

		#region Public methods

		#endregion

		#region Private methods

		#endregion

		#region MonoBehaviour functions

		// Use this for initialization
		void Start () {
			material = GetComponent<MeshRenderer>().material;
		}
	
		// Update is called once per frame
		void Update () {
			RenderTexture rt = null;
			switch (mode)
			{
				case Mode.Position:
					rt = shape.PositionRT;
					break;
				case Mode.Normal:
					rt = shape.NormalRT;
					break;
				case Mode.Velocity:
					rt = shape.VelocityRT;
					break;

			}

			if(rt != null) 
				material.SetTexture("_MainTex", rt);
		}
		#endregion
	}
		
}


