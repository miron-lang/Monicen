using UnityEngine;

public abstract class WaypointNavigatorBase : MonoBehaviour
{

    public WayPoint currentWaypoint;
    protected bool justExitCrosswalk;
    protected int diraction;
    protected bool isCrossing;
    protected abstract bool hasCharacter { get; }
    protected abstract bool hasDestinationReached { get; }
    protected abstract void LoceteDestination(Vector3 destination);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        diraction = Random.Range(0, 2);
        if (hasCharacter && currentWaypoint != null)
        {
            LoceteDestination(currentWaypoint.GetPosition(diraction));
        }
    }

    protected virtual void Update()
    {
        if (!hasCharacter || currentWaypoint == null || !hasDestinationReached)
        {
            return;
        }

        SelectNextWaypoint();

        if (currentWaypoint != null)
        {
            LoceteDestination(currentWaypoint.GetPosition(diraction));
        }
    }


    protected void SelectNextWaypoint()
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

    protected void SetCrosswalkDirection()
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

    protected bool ContinueCrosswalk()
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
