using UnityEngine;

public class Move : MonoBehaviour
{

    public float speed = 10f;
    public float rotationSpeed = 5f;

    public float jumpRegeneration = 0.5f;
    public float jumpStreanch = 250f;

    //public Animation anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            //anim.Play();
        }

         if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

         if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotationSpeed, 0);
            //anim.Play();
        }

         if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotationSpeed, 0);
            //anim.Play();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * jumpStreanch * Time.deltaTime);
        }

    }

}
