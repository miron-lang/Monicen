using UnityEngine;

public class SimpleGuncsterNavigator : WaypointNavigatorBase
{

    [Header("AI Guncster")]
    public SimpleGuncsterSkript charcater;

    protected override bool hasCharacter => charcater != null;
    protected override bool hasDestinationReached => charcater != null && charcater.destinationReached;

    private void Awake()
    {
        charcater = GetComponent<SimpleGuncsterSkript>();
    }

    protected override void LoceteDestination(Vector3 destination)
    {
        charcater.LoceteDestination(destination);
    }
}
