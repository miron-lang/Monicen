using UnityEngine;
using System.Collections.Generic;

public class WayPoint : MonoBehaviour
{
    public WayPoint previousWaypoint;
    public WayPoint nextWaypoint;

    [Range(0f, 5f)] public float width = 1f;
    public List<WayPoint> branches = new List<WayPoint>();

    [Range(0f, 1f)]
    public float branchRatio = 0.5f;

    public float incomingNPCs;

    public float waypointWidth;

    public Vector3 GetPosition(int diraction)
    {
        Vector3 minBounds = transform.position - transform.right * width / 2f;
        Vector3 maxBounds = transform.position + transform.right * width / 2f;
        return Vector3.Lerp(minBounds, maxBounds, Random.Range(0f, 1f));
    }
}