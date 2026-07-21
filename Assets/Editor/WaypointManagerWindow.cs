using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WayPointMenedgerWindow : EditorWindow
{
    [MenuItem("Waypoint/Waypoints Tools")]
    public static void ShowWindow()
    {
        GetWindow<WayPointMenedgerWindow>("Waypoints Editor Tools");
    }

    public Transform waypointOrigin;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("waypointOrigin"));

        if (waypointOrigin == null)
        {
            EditorGUILayout.HelpBox("Please assign a waypoint origin transform.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            CreateButtons();
            EditorGUILayout.EndVertical();
        }

        // Slider for waypointWidth of the selected WayPoint
        if (Selection.activeGameObject != null)
        {
            WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();
            if (selectedWaypoint != null)
            {
                SerializedObject wpSerialized = new SerializedObject(selectedWaypoint);
                SerializedProperty widthProp = wpSerialized.FindProperty("waypointWidth");
                if (widthProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Slider(widthProp, 0f, 10f, "Waypoint Width");
                    if (EditorGUI.EndChangeCheck())
                    {
                        wpSerialized.ApplyModifiedProperties();
                    }
                }
            }
        }

        obj.ApplyModifiedProperties();
    }

    void CreateButtons()
    {
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if (GUILayout.Button("Delete Waypoint"))
        {
            DeleteWaypoint();
        }
        if (GUILayout.Button("Previus Waypoint"))
        {
            PreviusWaypoint();
        }
        if (GUILayout.Button("Next Waypoint"))
        {
            NextWaypoint();
        }
        if (GUILayout.Button("Add Branch"))
        {
            AddBranch();
        }
        if (GUILayout.Button("Left"))
        {
            Selection.activeObject.GetComponent<WayPoint>().transform.Rotate(0, -45, 0);
        }
        if (GUILayout.Button("Right"))
        {
            Selection.activeObject.GetComponent<WayPoint>().transform.Rotate(0, 45, 0);
        }
        if (GUILayout.Button("Empty Waypoint"))
        {
            EmptyWaypoint();
        }
        if (GUILayout.Button("Conect Waypoints"))
        {
            ConectWaypoints();
        }
    }

    void PreviusWaypoint()
    {
        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint newWaypoint = waypointObj.GetComponent<WayPoint>();
        WayPoint selectedWaypoint = Selection.activeObject.GetComponent<WayPoint>();

        waypointObj.transform.position = selectedWaypoint.transform.position;
        waypointObj.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.peviousWaypoint)
        {
            newWaypoint.peviousWaypoint = selectedWaypoint.peviousWaypoint;
            selectedWaypoint.peviousWaypoint.nextWaypoint = newWaypoint;
        }

        selectedWaypoint.peviousWaypoint = newWaypoint;
        newWaypoint.nextWaypoint = selectedWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeObject = newWaypoint.gameObject;
    }

    void NextWaypoint()
    {
        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint newWaypoint = waypointObj.GetComponent<WayPoint>();
        WayPoint selectedWaypoint = Selection.activeObject.GetComponent<WayPoint>();

        waypointObj.transform.position = selectedWaypoint.transform.position;
        waypointObj.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.nextWaypoint)
        {
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            selectedWaypoint.nextWaypoint.peviousWaypoint = newWaypoint;
        }
        newWaypoint.peviousWaypoint = selectedWaypoint;
        selectedWaypoint.nextWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeObject = newWaypoint.gameObject;
    }

    void DeleteWaypoint()
    {
        WayPoint selectedWaypoint = Selection.activeObject.GetComponent<WayPoint>();

        if (selectedWaypoint.nextWaypoint && selectedWaypoint.peviousWaypoint)
        {
            selectedWaypoint.nextWaypoint.peviousWaypoint = selectedWaypoint.peviousWaypoint;
            selectedWaypoint.peviousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    void AddBranch()
    {
        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint waypoint = waypointObj.GetComponent<WayPoint>();

        WayPoint selectedWaypoint = Selection.activeObject.GetComponent<WayPoint>();

        selectedWaypoint.branches.Add(waypoint);
        waypoint.branches.Add(selectedWaypoint);
        waypoint.transform.position = selectedWaypoint.transform.position;
        waypoint.transform.forward = selectedWaypoint.transform.forward;

        Selection.activeObject = waypoint.gameObject;
    }

    void CreateWaypoint()
    {
        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint waypoint = waypointObj.GetComponent<WayPoint>();
        if (waypointOrigin.childCount > 1)
        {
            waypoint.peviousWaypoint = waypointOrigin.GetChild(waypointOrigin.childCount - 2).GetComponent<WayPoint>();
            waypoint.peviousWaypoint.nextWaypoint = waypoint;
            waypoint.transform.position = waypoint.peviousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.peviousWaypoint.transform.forward;
            waypoint.waypointWidth = waypoint.peviousWaypoint.waypointWidth;
        }

        Selection.activeObject = waypoint.gameObject;
    }

    void EmptyWaypoint()
    {
        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint waypoint = waypointObj.GetComponent<WayPoint>();
        Selection.activeObject = waypoint.gameObject;
    }

    void ConectWaypoints()
    {
        // Get all selected WayPoints
        GameObject[] selectedObjects = Selection.gameObjects;
        List<WayPoint> selectedWaypoints = new List<WayPoint>();

        foreach (GameObject go in selectedObjects)
        {
            WayPoint wp = go.GetComponent<WayPoint>();
            if (wp != null)
                selectedWaypoints.Add(wp);
        }

        if (selectedWaypoints.Count != 2)
        {
            Debug.LogWarning("Please select exactly two WayPoints to connect.");
            return;
        }

        WayPoint waypointA = selectedWaypoints[0];
        WayPoint waypointB = selectedWaypoints[1];

        // Add each to the other's branch list (bidirectional branch connection)
        if (!waypointA.branches.Contains(waypointB))
            waypointA.branches.Add(waypointB);
        if (!waypointB.branches.Contains(waypointA))
            waypointB.branches.Add(waypointA);

        // Optional: mark as dirty for undo
        EditorUtility.SetDirty(waypointA);
        EditorUtility.SetDirty(waypointB);
    }

}