using UnityEngine;

public class SunDirection : MonoBehaviour
{
    public float rotatiotSpeed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left * rotatiotSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * rotatiotSpeed * Time.deltaTime);
    }
}
