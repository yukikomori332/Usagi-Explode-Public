using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MyProject
{
	[CustomEditor(typeof(MapGenerator))]
	public class MapEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			MapGenerator map = target as MapGenerator;

			base.OnInspectorGUI();

			serializedObject.Update();

			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			if (GUILayout.Button("Generate Map"))
			{
				map.GenerateMap();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
