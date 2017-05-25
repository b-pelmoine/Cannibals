using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMeshSwitch))]
public class NavMeshSwitchEditor : Editor {

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        NavMeshSwitch t = target as NavMeshSwitch;
        t.debug = EditorGUILayout.Toggle(t.debug);
        t.switchName = GUILayout.TextField(t.switchName);
        if (GUILayout.Button("Add"))
        {
            t.switchStates.Add(new NavMeshSwitch.SwitchState());
        }
        if (GUILayout.Button("Delete"))
        {
            t.switchStates.RemoveAt(t.switchStates.Count - 1);
        }

        foreach(NavMeshSwitch.SwitchState st in t.switchStates)
        {
            GUILayout.BeginHorizontal();
            st.name = EditorGUILayout.TextField("Switch State:",st.name);
            st.aera = EditorGUILayout.TextField("Aera Name", st.aera);
            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
        PrefabUtility.RecordPrefabInstancePropertyModifications(t);
    }
}
