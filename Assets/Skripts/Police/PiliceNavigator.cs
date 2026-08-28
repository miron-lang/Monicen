using UnityEngine;

public class PiliceNavigator : WaypointNavigatorBase
{

    [Header("AI Police")]
    public PoliceOfficer charcater;

    protected override bool hasCharacter => charcater != null;
    protected override bool hasDestinationReached => charcater != null && charcater.destinationReached;

    private void Awake()
    {
        charcater = GetComponent<PoliceOfficer>();
    }

    protected override void LoceteDestination(Vector3 destination)
    {
        charcater.LoceteDestination(destination);
    }
}