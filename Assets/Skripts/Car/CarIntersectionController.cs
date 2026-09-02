using UnityEngine;
using System.Collections.Generic;


public static class CarIntersectionController
{
    static readonly Dictionary<Vector3Int, CarWaypointNafigator> owners = new Dictionary<Vector3Int, CarWaypointNafigator>();
    static readonly Collider[] exitCheckResults = new Collider[24];

    // Возвращает true, если перекрёсток и выбранная выходная полоса свободны
    public static bool TryEnter(CarWaypointNafigator requester, WayPoint entryWaypoint, WayPoint exitWaypoint, int exitDiraction, float exitCheckRadius, LayerMask carMask)
    {
        Vector3Int key = GetIntersectionKey(entryWaypoint);

        if (owners.TryGetValue(key, out CarWaypointNafigator owner) && owner != null && owner != requester)
        {
            return false;
        }
        if (!IsExitFree(requester, exitWaypoint, exitDiraction, exitCheckRadius, carMask))
        {
            return false;
        }
        owners[key] = requester;
        return true;
    }
    
    static bool IsExitFree(CarWaypointNafigator requester, WayPoint exitWaypoint, int exitDiraction, float radius, LayerMask carMask)
    {
        Vector3 exitPosition = exitWaypoint.GetPosition(exitDiraction);

        int hitCount = Physics.OverlapSphereNonAlloc(exitPosition, radius, exitCheckResults, carMask, QueryTriggerInteraction.Ignore);
        
        for (int i = 0; i < hitCount; i++)
        {
            CarWaypointNafigator otherCar = exitCheckResults[i].GetComponentInParent<CarWaypointNafigator>();

            if (otherCar != null && otherCar != requester)
            {
                return false;
            }
        }
        return true;
    }

    static Vector3Int GetIntersectionKey(WayPoint waypoint)
    {
        Vector3 center = waypoint.transform.position;
        int coint = 1;

        if (waypoint.branches != null)
        {
            for (int i = 0; i < waypoint.branches.Count; i++)
            {
                if (waypoint.branches[i] == null)
                {
                    continue;
                }

                center += waypoint.branches[i].transform.position;
                coint++;
            }
        }
        center /= coint;
        return Vector3Int.RoundToInt(center);
    }

    // Освобождает только тот перекрёсток, владельцем которого была эта бибика
    public static void Leave(CarWaypointNafigator requster, WayPoint intersectionWaypoint)
    {
        Vector3Int key = GetIntersectionKey(intersectionWaypoint);

        if (owners.TryGetValue(key, out CarWaypointNafigator owner) && owner == requster)
        {
            owners.Remove(key);
        }
    }
}
