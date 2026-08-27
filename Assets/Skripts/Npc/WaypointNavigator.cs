using UnityEngine;

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
}