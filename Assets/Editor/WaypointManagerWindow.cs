using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Waypoint/Waypoints Tools")]
    public static void ShowWindow()
    {
        GetWindow<WaypointManagerWindow>("Waypoints Editor Tools");
    }

    public Transform waypointOrigin;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("waypointOrigin"));

        if (waypointOrigin == null)
        {
            EditorGUILayout.HelpBox("Пожалуйста, перетащи сюда пустой GameObject (родитель вейпоинтов).", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("Create Waypoint (F1)")) CreateWaypoint();
            if (GUILayout.Button("Delete Waypoint (F2)")) DeleteWaypoint();
            if (GUILayout.Button("Previous Waypoint (F8)")) PreviousWaypoint();
            if (GUILayout.Button("Next Waypoint (F9)")) NextWaypoint();
            if (GUILayout.Button("Add Branch (F3)")) AddBranch();
            if (GUILayout.Button("Rotate Left (F5)")) RotateLeft();
            if (GUILayout.Button("Rotate Right (F6)")) RotateRight();
            if (GUILayout.Button("Empty Waypoint (F7)")) EmptyWaypoint();
            if (GUILayout.Button("Connect View Branches (F4)")) ConnectWaypointsBranches();
            if (GUILayout.Button("Connect Waypoints (F10)")) ConnectWaypointsDirect();
            EditorGUILayout.EndVertical();
        }

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

    [MenuItem("Waypoint/Actions/Create Waypoint _F1")]
    public static void CallCreate() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().CreateWaypoint(); }

    [MenuItem("Waypoint/Actions/Delete Waypoint _F2")]
    public static void CallDelete() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().DeleteWaypoint(); }

    [MenuItem("Waypoint/Actions/Add Branch _F3")]
    public static void CallAddBranch() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().AddBranch(); }

    [MenuItem("Waypoint/Actions/Connect View Branches _F4")]
    public static void CallConnectBranches() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().ConnectWaypointsBranches(); }

    [MenuItem("Waypoint/Actions/Rotate Left _F5")]
    public static void CallRotateLeft() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().RotateLeft(); }

    [MenuItem("Waypoint/Actions/Rotate Right _F6")]
    public static void CallRotateRight() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().RotateRight(); }

    [MenuItem("Waypoint/Actions/Empty Waypoint _F7")]
    public static void CallEmpty() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().EmptyWaypoint(); }

    [MenuItem("Waypoint/Actions/Previous Waypoint _F8")]
    public static void CallPrevious() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().PreviousWaypoint(); }

    [MenuItem("Waypoint/Actions/Next Waypoint _F9")]
    public static void CallNext() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().NextWaypoint(); }

    [MenuItem("Waypoint/Actions/Connect Waypoints _F10")]
    public static void CallConnectDirect() { if (HasOpenInstances<WaypointManagerWindow>()) GetWindow<WaypointManagerWindow>().ConnectWaypointsDirect(); }


    void CreateWaypoint()
    {
        if (waypointOrigin == null) return;

        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint waypoint = waypointObj.GetComponent<WayPoint>();

        if (waypointOrigin.childCount > 1)
        {
            waypoint.previousWaypoint = waypointOrigin.GetChild(waypointOrigin.childCount - 2).GetComponent<WayPoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;
            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
            waypoint.waypointWidth = waypoint.previousWaypoint.waypointWidth;
        }

        Selection.activeGameObject = waypointObj;
    }

    void DeleteWaypoint()
    {
        if (Selection.activeGameObject == null) return;
        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();
        if (selectedWaypoint == null) return;

        Undo.IncrementCurrentGroup();
        int groupIndex = Undo.GetCurrentGroup();

        if (selectedWaypoint.previousWaypoint != null)
        {
            Undo.RecordObject(selectedWaypoint.previousWaypoint, "Delete Waypoint Connection");
            selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            EditorUtility.SetDirty(selectedWaypoint.previousWaypoint);
        }

        if (selectedWaypoint.nextWaypoint != null)
        {
            Undo.RecordObject(selectedWaypoint.nextWaypoint, "Delete Waypoint Connection");
            selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            EditorUtility.SetDirty(selectedWaypoint.nextWaypoint);
        }

        if (selectedWaypoint.branches != null)
        {
            foreach (WayPoint branch in selectedWaypoint.branches)
            {
                if (branch != null && branch.branches.Contains(selectedWaypoint))
                {
                    Undo.RecordObject(branch, "Clean Branch Connection");
                    branch.branches.Remove(selectedWaypoint);
                    EditorUtility.SetDirty(branch);
                }
            }
        }

        Undo.DestroyObjectImmediate(selectedWaypoint.gameObject);
        Undo.CollapseUndoOperations(groupIndex);
    }

    void PreviousWaypoint()
    {
        if (Selection.activeGameObject == null || waypointOrigin == null) return;
        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();
        if (selectedWaypoint == null) return;

        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint newWaypoint = waypointObj.GetComponent<WayPoint>();

        waypointObj.transform.position = selectedWaypoint.transform.position;
        waypointObj.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.previousWaypoint)
        {
            newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
        }

        selectedWaypoint.previousWaypoint = newWaypoint;
        newWaypoint.nextWaypoint = selectedWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeGameObject = waypointObj;
    }

    void NextWaypoint()
    {
        if (Selection.activeGameObject == null || waypointOrigin == null) return;
        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();
        if (selectedWaypoint == null) return;

        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint newWaypoint = waypointObj.GetComponent<WayPoint>();

        waypointObj.transform.position = selectedWaypoint.transform.position;
        waypointObj.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.nextWaypoint)
        {
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            selectedWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
        }
        newWaypoint.previousWaypoint = selectedWaypoint;
        selectedWaypoint.nextWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeGameObject = waypointObj;
    }

    void AddBranch()
    {
        if (Selection.activeGameObject == null || waypointOrigin == null) return;
        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();
        if (selectedWaypoint == null) return;

        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        WayPoint waypoint = waypointObj.GetComponent<WayPoint>();

        selectedWaypoint.branches.Add(waypoint);
        waypoint.branches.Add(selectedWaypoint);
        waypoint.transform.position = selectedWaypoint.transform.position;
        waypoint.transform.forward = selectedWaypoint.transform.forward;

        Selection.activeGameObject = waypointObj;
    }

    void RotateLeft()
    {
        if (Selection.activeGameObject == null) return;
        WayPoint wp = Selection.activeGameObject.GetComponent<WayPoint>();
        if (wp != null) wp.transform.Rotate(0, -45, 0);
    }

    void RotateRight()
    {
        if (Selection.activeGameObject == null) return;
        WayPoint wp = Selection.activeGameObject.GetComponent<WayPoint>();
        if (wp != null) wp.transform.Rotate(0, 45, 0);
    }

    void EmptyWaypoint()
    {
        if (waypointOrigin == null) return;
        GameObject waypointObj = new GameObject("waypoint " + waypointOrigin.childCount, typeof(WayPoint));
        waypointObj.transform.SetParent(waypointOrigin, false);
        Selection.activeGameObject = waypointObj;
    }

    void ConnectWaypointsBranches()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        List<WayPoint> selectedWaypoints = new List<WayPoint>();

        foreach (GameObject go in selectedObjects)
        {
            WayPoint wp = go.GetComponent<WayPoint>();
            if (wp != null) selectedWaypoints.Add(wp);
        }

        if (selectedWaypoints.Count != 2) return;

        WayPoint waypointA = selectedWaypoints[0];
        WayPoint waypointB = selectedWaypoints[1];

        Undo.RecordObject(waypointA, "Connect Waypoint Branches");
        Undo.RecordObject(waypointB, "Connect Waypoint Branches");

        if (!waypointA.branches.Contains(waypointB)) waypointA.branches.Add(waypointB);
        if (!waypointB.branches.Contains(waypointA)) waypointB.branches.Add(waypointA);

        EditorUtility.SetDirty(waypointA);
        EditorUtility.SetDirty(waypointB);

        SceneView.RepaintAll();
    }

    void ConnectWaypointsDirect()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        List<WayPoint> selectedWaypoints = new List<WayPoint>();

        foreach (GameObject go in selectedObjects)
        {
            WayPoint wp = go.GetComponent<WayPoint>();
            if (wp != null) selectedWaypoints.Add(wp);
        }

        if (selectedWaypoints.Count != 2) return;

        WayPoint wp1 = selectedWaypoints[0];
        WayPoint wp2 = selectedWaypoints[1];

        Undo.RecordObject(wp1, "Connect Waypoints Direct");
        Undo.RecordObject(wp2, "Connect Waypoints Direct");

        wp1.nextWaypoint = wp2;
        wp2.previousWaypoint = wp1; // Возвращена "родная" опечатка

        EditorUtility.SetDirty(wp1);
        EditorUtility.SetDirty(wp2);

        SceneView.RepaintAll();
    }
}