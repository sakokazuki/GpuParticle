using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKZ.GPUParticle {
	
	public class TextureEmitterTest : MonoBehaviour {
		#region Public variables

		#endregion

		#region Private variables
		[SerializeField]
		TextureEmitter emitter;

		[SerializeField]
		Texture positionTexture;
		[SerializeField]
		Texture velocityTexture;
		#endregion

		#region Public methods

		#endregion

		#region Private methods
		void Emit()
		{
			emitter.SetTexture(positionTexture, velocityTexture);
			emitter.Emit();
		}
		#endregion

		#region MonoBehaviour functions

		private void OnGUI()
		{
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fontSize = 30;
			if (GUI.Button(new Rect(10, 10, 150, 100), "Emit", style))
			{
				Emit();
			}
		}
		#endregion
	}
		
}


