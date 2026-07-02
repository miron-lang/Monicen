using UnityEngine;

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

    void Start()
    {
        diraction = Mathf.RoundToInt(Random.Range(0f, 1f));
        if (currentWaypoint != null && car != null)
        {
            car.LoceteDesination(currentWaypoint.GetPosition(diraction));
        }
    }

    void Update()
    {
        if (currentWaypoint == null || car == null) return;

        // Корректное условие: переключаем точку только когда доехали
        if (car.destinationReached)
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
                if (diraction == 0)
                {
                    if (currentWaypoint.previousWaypoint != null)
                        currentWaypoint = currentWaypoint.previousWaypoint;
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
                        currentWaypoint = currentWaypoint.previousWaypoint;
                        diraction = 0;
                    }
                }
            }

            // Назначаем новую цель один раз после выбора новой точки
            if (currentWaypoint != null)
            {
                car.LoceteDesination(currentWaypoint.GetPosition(diraction));
            }
        }
    }
}