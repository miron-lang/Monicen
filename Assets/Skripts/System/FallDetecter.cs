using Unity.VisualScripting;
using UnityEngine;

public class FallDetecter : MonoBehaviour
{

    public GameObject respawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.transform.tag == "Player")
        {
            hit.transform.position = respawn.transform.position;
        }
    }

}
