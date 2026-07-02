using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(WayPoint waypoint, GizmoType gizmoType)
    {
        if (waypoint == null) return;

        bool isSelected = (gizmoType & GizmoType.Selected) != 0;

        Gizmos.color = isSelected ? Color.blue : Color.blue * 0.5f;
        Gizmos.DrawSphere(waypoint.transform.position, 0.2f);

        Gizmos.color = Color.white;
        Vector3 myRightPos = waypoint.GetPosition(1);
        Vector3 myLeftPos = waypoint.GetPosition(0);
        Gizmos.DrawLine(myLeftPos, myRightPos);

        // œ–¿¬¿ﬂ “–¿——¿ (¬œ≈–≈ƒ / «≈À≈Õ¿ﬂ)
        if (waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 nextRightPos = waypoint.nextWaypoint.GetPosition(1);
            Gizmos.DrawLine(myRightPos, nextRightPos);
            DrawArrowAtEnd(myRightPos, nextRightPos);
        }

        // À≈¬¿ﬂ “–¿——¿ (Õ¿«¿ƒ /  –¿—Õ¿ﬂ)
        if (waypoint.previousWaypoint != null) // —ÌÓ‚‡ Ò ÓÔÂ˜‡ÚÍÓÈ!
        {
            Gizmos.color = Color.red;
            Vector3 prevLeftPos = waypoint.previousWaypoint.GetPosition(0); // —ÌÓ‚‡ Ò ÓÔÂ˜‡ÚÍÓÈ!
            Gizmos.DrawLine(myLeftPos, prevLeftPos);
            DrawArrowAtEnd(myLeftPos, prevLeftPos);
        }

        // –¿«¬»À » (∆≈À“€≈)
        if (waypoint.branches != null)
        {
            Gizmos.color = Color.yellow;
            foreach (WayPoint branch in waypoint.branches)
            {
                if (branch != null)
                {
                    Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
                    DrawArrowAtEnd(waypoint.transform.position, branch.transform.position);
                }
            }
        }
    }

    private static void DrawArrowAtEnd(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        if (direction == Vector3.zero) return;

        Vector3 arrowTip = to;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 wingRight = lookRotation * Quaternion.Euler(0, 145, 0) * Vector3.forward;
        Vector3 wingLeft = lookRotation * Quaternion.Euler(0, -145, 0) * Vector3.forward;

        float arrowLength = 0.5f;

        Gizmos.DrawLine(arrowTip, arrowTip + wingRight * arrowLength);
        Gizmos.DrawLine(arrowTip, arrowTip + wingLeft * arrowLength);
    }
}