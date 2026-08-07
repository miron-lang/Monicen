using UnityEngine;

public class MiniMapSkript : MonoBehaviour
{
    public Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPosition = playerTransform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
