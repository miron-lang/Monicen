using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEditor;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{

    [Header("AI Âàñÿ")]
    public CharacterNavigatorScript charcater;
    public WayPoint currentWaypoint;

    int diraction;

    private void Awake()
    {
        charcater = GetComponent<CharacterNavigatorScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diraction = Mathf.RoundToInt(Random.Range(0f, 1f));
        charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
    }

    // Update is called once per frame
    void Update()
    {

        bool shouldBranch = false;

        if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
        {
            shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio ? true : false;
        }

        if (shouldBranch)
        {
            currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
        }

        else
        {
            if (charcater.destinationReached)
            {
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
        charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
    }
}