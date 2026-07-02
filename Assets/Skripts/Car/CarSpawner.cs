using System.Collections;
using UnityEngine;

public class CarSpawner : MonoBehaviour
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
        // Если забыли перетащить контейнер или там нет точек — просто выходим, чтобы не было ошибок
        if (waypointContainer == null || waypointContainer.childCount == 0)
        {
            Debug.LogWarning("CarSpawner: Укажи Waypoint Container в инспекторе!");
            yield break;
        }

        int count = 0;
        while (count < aiToSpawn)
        {
            int randomIndex = Random.Range(0, aiPrefabs.Length);
            GameObject obj = Instantiate(aiPrefabs[randomIndex]);

            // Исправлено: берем точку из контейнера вейпоинтов и убрали "- 1"
            Transform child = waypointContainer.GetChild(Random.Range(0, waypointContainer.childCount));

            // Назначаем вейпоинт машине
            var navigator = obj.GetComponent<CarWaypointNafigator>();
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