using UnityEngine;

public class PiliceNavigator : MonoBehaviour
{
    [Header("AI Police")]
    public PoliceOfficer charcater;
    public WayPoint currentWaypoint;

    int diraction;

    private void Awake()
    {
        charcater = GetComponent<PoliceOfficer>();
    }

    void Start()
    {
        diraction = Mathf.RoundToInt(Random.Range(0f, 1f));
        if (currentWaypoint != null && charcater != null)
        {
            charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
        }
    }

    void Update()
    {
        if (currentWaypoint == null || charcater == null) return;

        // Ждем завершения пути до вейпоинта
        if (charcater.destinationReached)
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

            // Установка следующей цели
            if (currentWaypoint != null)
            {
                charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
            }
        }
    }
}