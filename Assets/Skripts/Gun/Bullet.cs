using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float lifeTime = 1f;
    public float speed = 2500f;
    public float downSpeed = 5f;

    public Vector3 target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(target * speed * Time.deltaTime);
        transform.Translate(Vector3.down * downSpeed * Time.deltaTime);
    }
}  
