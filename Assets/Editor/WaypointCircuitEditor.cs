using UnityEngine;

public class WaypointCircuit : MonoBehaviour
{
    [Header("Настройки новых вейпоинтов")]
    [Range(0f, 10f)] public float defaultWidth = 2f;

    [Header("Диагностика системы путей")]
    public WayPoint lastCreatedWaypoint;

    private void OnValidate()
    {
        if (transform.childCount > 0 && lastCreatedWaypoint == null)
        {
            lastCreatedWaypoint = transform.GetChild(transform.childCount - 1).GetComponent<WayPoint>();
        }
    }
}