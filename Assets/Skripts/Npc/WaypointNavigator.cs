using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEditor;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{

    [Header("AI Âàñÿ")]
    public CharacterNavigatorScript01 charcater;
    public WayPoint currentWaypoint;

    int diraction;

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
        bool shouldBranch = currentWaypoint.branches != null && currentWaypoint.branches.Count > 0 && Random.value <= currentWaypoint.branchRatio;

        if (shouldBranch)
        {
            currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
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
}