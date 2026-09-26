using UnityEngine;

public class MiniMapSkript : MonoBehaviour
{
    public Transform playerTransform;
    public Vechicle car;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!car.inOpened)
        {
            Vector3 newPosition = playerTransform.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
        else
        {
            Vector3 newPosition = car.transform.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
}
