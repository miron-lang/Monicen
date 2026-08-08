using UnityEngine;

public class SmoothCarCamera : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0.3f, 2.3f, -6.5f);
    public float positionSmoothTime = 0.25f;
    public float rotationSmoothSpeed = 5f;

    private Vector3 velosity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.TransformPoint(offset);//преоброзует локалную смешеню в мировую позицыю
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velosity, positionSmoothTime);//ѕлавно перемещ€ют
        Vector3 lookPosition = target.position + Vector3.up * 1.2f;
        Quaternion disaredRotation = Quaternion.LookRotation(lookPosition - transform.position);//”казывоет куда смотерть
        transform.rotation = Quaternion.Slerp(transform.rotation, disaredRotation, rotationSmoothSpeed * Time.deltaTime);//ѕлавно вращ€ет
    }
}
