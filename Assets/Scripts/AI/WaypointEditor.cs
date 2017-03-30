using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoint))]
//[CanEditMultipleObjects]
public class WaypointEditor : Editor {

    public SerializedProperty waypoint;
    public SerializedProperty links;

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
        serializedObject.Update();
        var t = (target as Waypoint);
        if(selectedPoint>=0 && selectedPoint < t.points.Count)
        {
            Vector3 newPosition = EditorGUILayout.Vector3Field("Position", t.points[selectedPoint].position);
            if (newPosition != t.points[selectedPoint].position)
            {
                t.points[selectedPoint].position = newPosition;
                Repaint();
            }
            EditorGUILayout.Space();
            for(int i = 0; i < t.points[selectedPoint].links.Count; i++)
            {
                t.points[selectedPoint].links[i] = EditorGUILayout.IntField(t.points[selectedPoint].links[i]);
            }
        }
        serializedObject.ApplyModifiedProperties();
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
            Handles.FreeMoveHandle(t.points[i].position, Quaternion.identity, .5f, new Vector3(.5f, .5f, .5f), Handles.SphereCap);
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
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = controlID;
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(worldRay, out hitInfo))
                    {
                        changed = true;
                        t.Add(hitInfo.point);
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
                    for(int j= 0;j < wpoint.links.Count;j++)
                    {
                        var p = wpoint.links[j];
                        Handles.color = Color.red;
                        Vector3 v = t.points[p].position - wpoint.position;
                        Vector3 arrow_position = wpoint.position + v*0.6f;
                        Vector3 v2 = Quaternion.Euler(0, 15, 0)*(-v.normalized/2);
                        Vector3 v3 = Quaternion.Euler(0, -15, 0) * (-v.normalized/2);
                        Handles.DrawLine(wpoint.position, t.points[p].position);
                        Handles.color = Color.magenta;
                        Handles.DrawLine(arrow_position, arrow_position+v2);
                        Handles.DrawLine(arrow_position, arrow_position+v3);
                        Handles.color = Color.white;
                    }
                }
                if (drag)
                    Handles.DrawLine(t.points[startPoint].position, castRay());
                break;

            default:
                break;
        }
        if (changed)
        {
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
}
