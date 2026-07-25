using UnityEngine;

public class SimpleGuncsterNavigator : MonoBehaviour
{

    [Header("AI Guncster")]
    public SimpleGuncsterSkript charcater;
    public WayPoint currentWaypoint;

    int diraction;

    private void Awake()
    {
        charcater = GetComponent<SimpleGuncsterSkript>();
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

        if (currentWaypoint && currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
        {
            float randomBranch = Random.Range(0f, 1f);
            shouldBranch = randomBranch <= currentWaypoint.branchRatio ? true : false;
        }

        if (shouldBranch)
        {
            currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
        }

        else
        {
            if (charcater.destinationReached)
            {
                if (diraction == 0)
                    if (currentWaypoint.peviousWaypoint != null)
                        currentWaypoint = currentWaypoint.peviousWaypoint;

                    else
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        diraction = 1;
                    }

                else
                  if (currentWaypoint.nextWaypoint != null)
                    currentWaypoint = currentWaypoint.nextWaypoint;

                else
                {
                    currentWaypoint = currentWaypoint.peviousWaypoint;
                    diraction = 0;
                }

            }
        }
        charcater.LoceteDesination(currentWaypoint.GetPosition(diraction));
    }
}
