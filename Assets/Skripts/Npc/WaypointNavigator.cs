using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    [Header("AI Вася")]
    public CharacterNavigatorScript character;

    //private WayPoint _currentWaypoint;
    public WayPoint currentWaypoint;
    //{
    //    get => _currentWaypoint;
    //    set
    //    {
    //        if (_currentWaypoint != null) _currentWaypoint.incomingNPCs--;
    //        _currentWaypoint = value;
    //        if (_currentWaypoint != null) _currentWaypoint.incomingNPCs++;
    //    }
    //}

    int direction;

    private void Awake()
    {
        character = GetComponent<CharacterNavigatorScript>();
    }

    void Start()
    {
        direction = Random.Range(0, 2);
        if (currentWaypoint != null)
            character.LocateDestination(currentWaypoint.GetPosition(direction));
    }

    void Update()
    {
        if (currentWaypoint == null || character == null) return;

        if (character.destinationReached)
        {
            bool shouldBranch = false;
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
            }
            else
            {
                if (direction == 0)
                {
                    if (currentWaypoint.previousWaypoint != null) // Снова с опечаткой!
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    else
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction = 1;
                    }
                }
                else
                {
                    if (currentWaypoint.nextWaypoint != null)
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    else
                    {
                        if (currentWaypoint.previousWaypoint != null) // Снова с опечаткой!
                            currentWaypoint = currentWaypoint.previousWaypoint;
                        direction = 0;
                    }
                }
            }

            if (currentWaypoint != null)
                character.LocateDestination(currentWaypoint.GetPosition(direction));
        }
    }

    //private void OnDestroy()
    //{
    //    if (currentWaypoint != null) currentWaypoint.incomingNPCs--;
    //}
}