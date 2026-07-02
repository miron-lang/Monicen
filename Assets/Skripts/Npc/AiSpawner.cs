using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject[] aiPrefabs;
    public int aiToSpawn = 10;
    public float spawnDelay = 0.1f;

    [Header("Сеть вейпоинтов")]
    [SerializeField] private Transform waypointContainer;

    [Header("Настройки радиуса (Умный спавн)")]
    public Transform playerTransform;      // Сюда перетащи Игрока
    public float minSpawnRadius = 15f;     // Ближе этого расстояния к игроку спавнить нельзя (чтобы не из воздуха)
    public float maxSpawnRadius = 50f;     // Дальше этого расстояния спавнить нет смысла

    void Start()
    {
        if (aiPrefabs == null || aiPrefabs.Length == 0)
        {
            Debug.LogError("Забыл добавить префабы ИИ на объекте " + gameObject.name);
            return;
        }

        if (waypointContainer == null || waypointContainer.childCount == 0)
        {
            Debug.LogError("Забыл привязать Waypoint Container!");
            return;
        }

        if (playerTransform == null)
        {
            // Попробуем найти игрока по тегу, если ты забыл его перетащить в инспектор
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else Debug.LogWarning("Игрок не найден! Спавнер будет игнорировать радиус и смотреть только на плотность NPC.");
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        int count = 0;

        while (count < aiToSpawn)
        {
            WayPoint bestWaypoint = FindBestWaypoint();

            if (bestWaypoint != null)
            {
                int randomPrefabIndex = Random.Range(0, aiPrefabs.Length);
                GameObject npc = Instantiate(aiPrefabs[randomPrefabIndex]);

                npc.transform.position = bestWaypoint.transform.position;

                WaypointNavigator navigator = npc.GetComponent<WaypointNavigator>();
                if (navigator != null)
                {
                    navigator.currentWaypoint = bestWaypoint;
                }

                yield return new WaitForSeconds(spawnDelay);
                count++;
            }
            else
            {
                // Если в радиусе игрока вообще нет точек (например, он ушел в чистое поле), 
                // подождем секунду и попробуем снова, чтобы не вешать цикл
                yield return new WaitForSeconds(1f);
            }
        }
    }

    WayPoint FindBestWaypoint()
    {
        List<WayPoint> validWaypoints = new List<WayPoint>();
        int totalWaypoints = waypointContainer.childCount;

        // ШАГ 1: Фильтруем точки по радиусу от игрока
        for (int i = 0; i < totalWaypoints; i++)
        {
            Transform wpTransform = waypointContainer.GetChild(i);
            WayPoint wp = wpTransform.GetComponent<WayPoint>();

            if (wp == null) continue;

            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(wpTransform.position, playerTransform.position);

                // Если точка входит в наш "круг обзора" вокруг игрока
                if (distanceToPlayer >= minSpawnRadius && distanceToPlayer <= maxSpawnRadius)
                {
                    validWaypoints.Add(wp);
                }
            }
            else
            {
                // Если игрока на сцене нет, берем вообще все точки
                validWaypoints.Add(wp);
            }
        }

        // Если в радиусе ничего не нашли, берем абсолютно все точки контейнера как запасной вариант
        if (validWaypoints.Count == 0)
        {
            for (int i = 0; i < totalWaypoints; i++)
            {
                WayPoint wp = waypointContainer.GetChild(i).GetComponent<WayPoint>();
                if (wp != null) validWaypoints.Add(wp);
            }
        }

        if (validWaypoints.Count == 0) return null;

        // ШАГ 2: Ищем точки с минимальным количеством входящих NPC
        int minNPCs = int.MaxValue;
        foreach (WayPoint wp in validWaypoints)
        {
            if (wp.incomingNPCs < minNPCs)
            {
                //minNPCs = wp.incomingNPCs;
            }
        }

        // Собираем все точки, у которых этот минимальный счетчик (чтобы был элемент рандома среди пустых точек)
        List<WayPoint> bestWaypoints = new List<WayPoint>();
        foreach (WayPoint wp in validWaypoints)
        {
            if (wp.incomingNPCs == minNPCs)
            {
                bestWaypoints.Add(wp);
            }
        }

        // Возвращаем случайную точку из списка самых ненаселенных
        return bestWaypoints[Random.Range(0, bestWaypoints.Count)];
    }
}