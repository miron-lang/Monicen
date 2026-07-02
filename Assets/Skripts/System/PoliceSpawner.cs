using System.Collections;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] aiPrefabs;
    [SerializeField] int aiToSpawn;

    // Ссылка на твой объект с вейпоинтами из инспектора
    [SerializeField] Transform waypointContainer;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        // Защита от пустой ссылки на точки
        if (waypointContainer == null || waypointContainer.childCount == 0)
        {
            Debug.LogWarning("PoliceSpawner: Укажи Waypoint Container в инспекторе!");
            yield break;
        }

        int count = 0;
        while (count < aiToSpawn)
        {
            int randomIndex = Random.Range(0, aiPrefabs.Length);
            GameObject obj = Instantiate(aiPrefabs[randomIndex]);

            // Исправлено: берем точку из контейнера вейпоинтов и убрали "- 1"
            Transform child = waypointContainer.GetChild(Random.Range(0, waypointContainer.childCount));

            // Назначаем вейпоинт полиции
            var navigator = obj.GetComponent<PiliceNavigator>();
            if (navigator != null)
            {
                navigator.currentWaypoint = child.GetComponent<WayPoint>();
            }

            obj.transform.position = child.position;

            yield return new WaitForSeconds(0.1f);
            count++;
        }
    }
}