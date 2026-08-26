using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [Header("Waypoint status")]
    public WayPoint peviousWaypoint;
    public WayPoint nextWaypoint;
    public bool isCrosswalk = false;

    [Range(0f, 10f)]
    public float waypointWidth = 1f;

    public List<WayPoint> branches = new List<WayPoint>();

    [Range(0f, 1f)]
    public float branchRatio = 0.5f;

    public Vector3 GetPosition(int diraction)
    {
        Vector3 minBound = transform.position + transform.right * waypointWidth / 2f;
        Vector3 maxBound = transform.position - transform.right * waypointWidth / 2f;
        return Vector3.Lerp(minBound, maxBound, diraction);
    }

}