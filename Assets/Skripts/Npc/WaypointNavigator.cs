using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

public class WaypointNavigator : MonoBehaviour
{

    [Header("AI Âàñÿ")]
    public CharacterNavigatorScript01 charcater;
    public WayPoint currentWaypoint;

    private bool justExitCrosswalk;

    int diraction;

    private bool isCrossing;

    private void Awake()
    {
        charcater = GetComponent<CharacterNavigatorScript01>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diraction = Random.Range(0, 2);
        charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
    }

    // Update is called once per frame
    void Update()
    {
        if (charcater == null || currentWaypoint == null || !charcater.destinationReached)
        {
            return;
        }

        SelectNextWaypoint();

        if (currentWaypoint != null)
        {
            charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
        }
    }

    void SelectNextWaypoint()
    {
        if (isCrossing && ContinueCrosswalk())
        {
            return;
        }
        bool skipBranch = justExitCrosswalk;
        if (skipBranch)
        {
            justExitCrosswalk = false;
            diraction = Random.Range(0, 2);
        }
        //if (diraction == 1)
        //{
        //    if (currentWaypoint.nextWaypoint != null && currentWaypoint.nextWaypoint.isCrosswalk)
        //    {
        //        currentWaypoint = currentWaypoint.nextWaypoint;
        //        return;
        //    }
        //    if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
        //    {
        //        currentWaypoint = currentWaypoint.branches[0];
        //        isCrossing = false;
        //        return;
        //    }
        //}
        //else
        //{
        //    if (currentWaypoint.peviousWaypoint != null && currentWaypoint.peviousWaypoint.isCrosswalk)
        //    {
        //        currentWaypoint = currentWaypoint.peviousWaypoint;
        //        return;
        //    }
        //    if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
        //    {
        //        currentWaypoint = currentWaypoint.branches[0];
        //        isCrossing = false;
        //        return;
        //    }
        //}
        bool shouldBranch = !skipBranch && currentWaypoint.branches != null && currentWaypoint.branches.Count > 0 && Random.value <= currentWaypoint.branchRatio;

        if (shouldBranch)
        {
            WayPoint selectedBranch = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
            currentWaypoint = selectedBranch;
            if (currentWaypoint != null && currentWaypoint.isCrosswalk)
            {
                isCrossing = true;
                SetCrosswalkDirection();
            }
            return;
        }

        if (diraction == 0)
        {
            if (currentWaypoint.peviousWaypoint != null)
                currentWaypoint = currentWaypoint.peviousWaypoint;

            else
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
                diraction = 1;
            }
        }

        else
        {
            if (currentWaypoint.nextWaypoint != null)
                currentWaypoint = currentWaypoint.nextWaypoint;

            else
            {
                currentWaypoint = currentWaypoint.peviousWaypoint;
                diraction = 0;
            }
        }

    }
    
    void SetCrosswalkDirection()
    {
        if (currentWaypoint.nextWaypoint != null && currentWaypoint.nextWaypoint.isCrosswalk)
        {
            diraction = 1;
        }
        else if (currentWaypoint.peviousWaypoint != null && currentWaypoint.peviousWaypoint.isCrosswalk)
        {
            diraction = 0;
        }
    }

    bool ContinueCrosswalk()
    {
        WayPoint nextCrosswalkPoint = diraction == 1 ? currentWaypoint.nextWaypoint : currentWaypoint.peviousWaypoint;

        if (nextCrosswalkPoint != null && nextCrosswalkPoint.isCrosswalk)
        {
            currentWaypoint = nextCrosswalkPoint;
            return true;
        }
        if (currentWaypoint.branches != null)
        {
            for (int i = 0; i < currentWaypoint.branches.Count; i++)
            {
                WayPoint extiWaypoin = currentWaypoint.branches[i];
                if (extiWaypoin != null && !extiWaypoin.isCrosswalk)
                {
                    currentWaypoint = extiWaypoin;
                    isCrossing = false;
                    justExitCrosswalk = true;
                    return true;
                }
            }
        }
        if (nextCrosswalkPoint != null)
        {
            currentWaypoint = nextCrosswalkPoint;
            isCrossing = false;
            justExitCrosswalk = true;
            return true;
        }
        isCrossing = false;
        return false;
    }
}