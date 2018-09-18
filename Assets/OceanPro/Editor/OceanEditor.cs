using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OceanPro
{
	[CustomEditor(typeof(Ocean))]
	public class OceanEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var t = (Ocean)target;

			EditorGUI.BeginChangeCheck();
			var mode = GUILayout.Toggle(t.wireframeMode, " Wireframe Mode");
			if(EditorGUI.EndChangeCheck())
			{
				t.SetWireframeMode(mode);
			}

			DrawDefaultInspector();
		}
	}
}