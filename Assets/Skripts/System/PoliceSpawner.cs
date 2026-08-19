using System.Collections;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    public GameObject[] aiPrefabs;
    public int aiToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < aiToSpawn)
        {
            int randomIndex = Random.Range(0, aiPrefabs.Length);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount));
            GameObject obj = Instantiate(aiPrefabs[randomIndex], child.position, child.rotation);
            obj.GetComponent<PiliceNavigator>().currentWaypoint = child.GetComponent<WayPoint>();
            yield return new WaitForSeconds(0.1f);
            count++;
        }
    }
}