using Unity.VisualScripting;
using UnityEngine;

public class FallDetecter : MonoBehaviour
{
    public Transform respawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = respawn.position;
    }

}
