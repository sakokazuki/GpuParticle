using UnityEditor;

namespace SKZ.GPULines
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LineRenderer))]
	public class LineRendererEditor : Editor
	{

		SerializedProperty parameter;

		// Use this for initialization
		void OnEnable()
		{
			parameter = serializedObject.FindProperty("parameter");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(parameter);

			serializedObject.ApplyModifiedProperties();

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
