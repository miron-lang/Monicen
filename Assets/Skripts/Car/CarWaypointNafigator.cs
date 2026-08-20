using UnityEngine;
using UnityEngine.TextCore.Text;

public class CarWaypointNafigator : MonoBehaviour
{
    [Header("AI Car")]
    public CarNavigator car;
    public WayPoint currentWaypoint;

    int diraction;

    private void Awake()
    {
        car = GetComponent<CarNavigator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diraction = Random.Range(0, 2);

        if (car == null || currentWaypoint == null)
        {
            return;
        }

        Vector3 startPosition = currentWaypoint.GetPosition(diraction);

        startPosition.y = transform.position.y;

        transform.position = startPosition;

        SelectNextWaypoint();

        if (currentWaypoint != null)
        {
            car.LoceteDesination(currentWaypoint.GetPosition(diraction));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (car == null || currentWaypoint == null || !car.destinationReached)
        {
            return;
        }

        SelectNextWaypoint();

        if (currentWaypoint != null)
        {
            car.LoceteDesination(currentWaypoint.GetPosition(diraction));
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