using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor {

	public override void OnInspectorGUI ()
	{
		
	
		Transform t = target as Transform;
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Reset Position", GUILayout.Width(20), GUILayout.Height(20))) {
			Undo.RecordObject (t, "Reset Position " + t.name);
			t.transform.localPosition = Vector3.zero;
		}
		EditorGUIUtility.labelWidth = 70;
		EditorGUIUtility.fieldWidth = 70;
		EditorGUI.indentLevel = 0;
		Vector3 position  = EditorGUILayout.Vector3Field("Position", t.localPosition);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Reset Rotation", GUILayout.Width(20), GUILayout.Height(20))) {
			Undo.RecordObject (t, "Reset Rotation " + t.name);
			t.transform.localRotation = Quaternion.identity;
		}
		Vector3 rotation  = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Reset Scale", GUILayout.Width(20), GUILayout.Height(20))) {
			Undo.RecordObject (t, "Reset Scale " + t.name);
			t.transform.localScale = Vector3.one;
		}
		Vector3 scale 	  = EditorGUILayout.Vector3Field("Scale", t.localScale);
		EditorGUILayout.EndHorizontal ();

		//EditorGUIUtility.LookLikeInspector
		EditorGUIUtility.labelWidth = 0;
		EditorGUIUtility.fieldWidth = 0;
		if (GUI.changed)
		{
			Undo.RecordObject (t, "Transform Change");
			t.localPosition = FixIfNaN(position);
			t.localEulerAngles = FixIfNaN(rotation);
			t.localScale = FixIfNaN(scale);
		}
		//base.OnInspectorGUI ();
	}
	private Vector3 FixIfNaN(Vector3 v)
	{
		if (float.IsNaN(v.x)) {
			v.x = 0;
		}if (float.IsNaN(v.y)) {
			v.y = 0;
		}if (float.IsNaN(v.z)) {
			v.z = 0;
		}
		return v;
	}
}
