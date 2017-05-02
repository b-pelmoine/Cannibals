using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Waypoint))]
//[CanEditMultipleObjects]
public class WaypointEditor : Editor {

    public SerializedProperty waypoint;
    public SerializedProperty links;

    bool _showPoints;
    

    int selectedPoint = -1;

    bool drag = false;
    bool changed = false;
    int startPoint = -1;
	void OnEnable()
    {
        waypoint = serializedObject.FindProperty("points");
        
    }

    public override void OnInspectorGUI()
    {
        List<bool> _showPointInfo = new List<bool>();
        for (int i = 0; i < (target as Waypoint).points.Count; i++)
            _showPointInfo.Add(false);
        serializedObject.Update();
        var t = (target as Waypoint);
        if(selectedPoint>=0 && selectedPoint < t.points.Count)
        {
            Vector3 newPosition = EditorGUILayout.Vector3Field("Position", t.points[selectedPoint].position);
            if (newPosition != t.points[selectedPoint].position)
            {
                t.points[selectedPoint].position = newPosition;
                changed = true;
            }
            EditorGUILayout.Space();
            for(int i = 0; i < t.points[selectedPoint].links.Count; i++)
            {
                t.points[selectedPoint].links[i] = EditorGUILayout.IntField(t.points[selectedPoint].links[i]);
            }
        }
        _showPoints = EditorGUILayout.Foldout(_showPoints, "Points list");
        if (_showPoints)
        {

            for (int i = 0; i < t.points.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(7);
                _showPointInfo[i] = EditorGUI.Foldout(EditorGUILayout.GetControlRect(GUILayout.MaxWidth(0)),_showPointInfo[i], GUIContent.none);
                string name = EditorGUILayout.TextField(t.points[i].tag);
                if (t.points[i].tag != null && !name.Equals(t.points[i].tag, System.StringComparison.Ordinal))
                {
                    t.points[i].tag = name;
                    changed = true;
                }
                if (GUILayout.Button("Erase"))
                {
                    t.points.RemoveAt(i);
                    _showPointInfo.RemoveAt(i);
                    changed = true;
                    i--;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5);
                EditorGUILayout.BeginVertical();
                
                if (_showPointInfo[i])
                {
                    Vector3 newPosition = EditorGUILayout.Vector3Field("Position", t.points[i].position);
                    if (newPosition != t.points[i].position)
                    {
                        t.points[i].position = newPosition;
                        changed = true;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("Clear"))
        {
            t.points.Clear();
            _showPointInfo.Clear();
            changed = true;
        }
        
        serializedObject.ApplyModifiedProperties();
        if (changed)
        {
            SceneView.RepaintAll();
            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            changed = false;
        }
    }

    public void OnSceneGUI()
    {
        var t = (target as Waypoint);

        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        int lastControlID;
        for (int i = 0; i < t.points.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Handles.color = Color.blue;
            lastControlID = GUIUtility.hotControl;
            Handles.Label(t.points[i].position, t.points[i].tag);
            Handles.FreeMoveHandle(t.points[i].position, Quaternion.identity, .5f, new Vector3(.5f, .5f, .5f), Handles.SphereHandleCap);
            Handles.color = Color.white;
            if (GUIUtility.hotControl != lastControlID)
            {
                selectedPoint = i;
                Repaint();
            }
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                t.points[i].position = castRay();
                //t.Update();
            }
        }

        switch (Event.current.type)
        {
            case EventType.mouseDown:
                if (Event.current.button == 0 && Event.current.alt==false)
                {
                    GUIUtility.hotControl = controlID;
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(worldRay, out hitInfo))
                    {
                        changed = true;
                        t.Add(hitInfo.point);
                        t.points[t.points.Count - 1].tag = "" + t.points.Count;
                        if (t.points.Count > 0)
                            t.AddLink(t.points.Count - 2, t.points.Count - 1);
                    }
                    HandleUtility.Repaint();
                    GUIUtility.hotControl = 0;
                    selectedPoint = -1;
                }
                else if(Event.current.button == 1)
                {
                    drag = true;
                    startPoint = nearestPoint();
                    GUIUtility.hotControl = controlID;
                }

                break;

            case EventType.mouseUp:
                if (drag)
                {
                    drag = false;
                    GUIUtility.hotControl = 0;
                    selectedPoint = -1;
                    int point = nearestPoint();
                    if (point != startPoint)
                        t.points[startPoint].links.Add(point);
                    HandleUtility.Repaint();
                    changed = true;
                }
                break;

            case EventType.keyDown:
                if (Event.current.keyCode == KeyCode.Delete && selectedPoint != -1)
                {
                    t.Delete(selectedPoint);
                    changed = true;
                    selectedPoint = -1;
                    Event.current.Use();
                }
                break;

            case EventType.mouseDrag:
                HandleUtility.Repaint();
                break;

            case EventType.Repaint:
                for(int i=0; i < t.points.Count;i++) 
                {
                    var wpoint = t.points[i];
                    var wnext = t.points[(i + 1) % t.points.Count];
                    drawArrow(wpoint.position, wnext.position);
                }
                if (drag)
                    Handles.DrawLine(t.points[startPoint].position, castRay());
                break;

            default:
                break;
        }
        if (changed)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Undo.RegisterCompleteObjectUndo(target, "Waypoint Changed");
            changed = false;
        }
    }

    Vector3 castRay()
    {
        RaycastHit hitInfo;
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if(Physics.Raycast(worldRay, out hitInfo)){
            return hitInfo.point;
        }
        return worldRay.origin+worldRay.direction*10;
    }

    int nearestPoint()
    {
        int result = -1;
        var t = (target as Waypoint);
        for(int i = 0; i < t.points.Count; i++)
        {
            if (result == -1 || Vector3.Distance(t.points[i].position, castRay()) < Vector3.Distance(t.points[result].position, castRay()))
                result = i;
        }
        return result;
    }

    private void drawArrow(Vector3 origin, Vector3 destination)
    {
        Handles.color = Color.red;
        Vector3 v = destination - origin;
        Vector3 arrow_position = origin + v * 0.6f;
        Vector3 v2 = Quaternion.Euler(0, 15, 0) * (-v.normalized / 2);
        Vector3 v3 = Quaternion.Euler(0, -15, 0) * (-v.normalized / 2);
        Handles.DrawLine(origin, destination);
        Handles.color = Color.magenta;
        Handles.DrawLine(arrow_position, arrow_position + v2);
        Handles.DrawLine(arrow_position, arrow_position + v3);
        Handles.color = Color.white;
    }
}
