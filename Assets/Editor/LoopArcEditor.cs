using UnityEngine;
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LoopArc))] 
public class LoopArcEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		LoopArc loop = (LoopArc)target;
		if(GUILayout.Button("Refresh"))
		{
			loop.EditorRefresh ();
		}
	}

}
