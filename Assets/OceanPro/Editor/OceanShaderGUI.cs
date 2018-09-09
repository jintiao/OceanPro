using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OceanShaderGUI : ShaderGUI
{
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		GUILayout.Label("123344");

		base.OnGUI(materialEditor, properties);
	}
}
