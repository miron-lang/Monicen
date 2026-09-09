using UnityEngine;
using UnityEngine.Splines;

public class WaypointNavigator : WaypointNavigatorBase
{

    [Header("AI Вася")]
    public CharacterNavigatorScript01 charcater;
    protected override bool hasCharacter => charcater != null;
    protected override bool hasDestinationReached => charcater != null && charcater.destinationReached;

    private void Awake()
    {
        charcater = GetComponent<CharacterNavigatorScript01>();
    }

    protected override void LoceteDestination(Vector3 destination)
    {
        charcater.LoceteDestination(destination);
    }

    protected override void Update()
    {
        if (charcater != null && charcater.IsEscaping())
        {
            if (charcater.destinationReached)
            {
                SelectEscapeWaypoint();
            }
            return;
        }

        base.Update();
    }

    void SelectEscapeWaypoint()
    {

    }

    public bool StartEscape(Vector3 escapeDirection)
    {
        if (charcater == null || currentWaypoint == null)
        {
            return false;
        }
        
        WayPoint nextWaypoint = currentWaypoint.nextWaypoint;
        WayPoint previusWaypoint = currentWaypoint.peviousWaypoint;
        float nextScore = GetEscapeScore(nextWaypoint, escapeDirection);
        float previusScore = GetEscapeScore(previusWaypoint, escapeDirection);

        if (nextScore == float.NegativeInfinity && previusScore == float.NegativeInfinity)
        {
            return false;
        }

        if (nextScore >= previusScore)
        {
            currentWaypoint = nextWaypoint;
            diraction = 1;
        }
        else
        {
            currentWaypoint = previusWaypoint;
            diraction = 0;
        }

        isCrossing = false;
        justExitCrosswalk = false;

        LoceteDestination(currentWaypoint.GetPosition(diraction));
        return true;
    }

    float GetEscapeScore(WayPoint candidate, Vector3 escapeDiraction)
    {
        if (candidate == null)
        {
            return float.NegativeInfinity;
        }

        Vector3 canditateDirection = candidate.transform.position - transform.position; 
        canditateDirection.y = 0;

        if (canditateDirection.sqrMagnitude < 0.01f)
        {
            return float.NegativeInfinity;
        }

        return Vector3.Dot(canditateDirection.normalized, escapeDiraction);
;
    }
}