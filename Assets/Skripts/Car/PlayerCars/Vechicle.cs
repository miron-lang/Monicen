using System.Xml.Serialization;
using UnityEngine;

public class Vechicle : MonoBehaviour
{

    public WheelCollider[] wheelsColliders;

    public Transform[] wheelsTransform;

    public Transform collidersEmpty;
    public Transform transformsEmpty;

    private float presentTurnAngle = 0f;
    private float presentAcceleration = 0f;

    public Transform doorPositon;

    private Rigidbody rb;

    [Header("Vechicle steering")]
    public float wheelsTorque;
    
    [Header("Vechicle engine")]
    public float accelerationForce;
    public float breakingForce;
    private float pressentBreakForce = 0f;
    public GameObject carCamera;

    [Header("Vechical security")]
    public Transform player;
    public bool inOpened = false;
    public float radius = 5f;

    [Header("Disable things")]
    public GameObject mainCamera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheelsColliders = collidersEmpty.GetComponentsInChildren<WheelCollider>();

        wheelsTransform = new Transform[transformsEmpty.childCount];
        for (int i = 0; i < transformsEmpty.childCount; i++)
        {
            wheelsTransform[i] = transformsEmpty.GetChild(i).transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rb.linearVelocity.magnitude * 3.6f + " κμ/χÿρ");
        if (Vector3.Distance(player.position, transform.position) <= radius)
        {
            if (Input.GetKeyDown(KeyCode.F) && !inOpened)
            {
                carCamera.gameObject.SetActive(true);

                inOpened = true;
                radius = 999999f;

                player.gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.F) && inOpened)
            {

                player.gameObject.SetActive(true);

                inOpened = false;
                player.transform.position = transform.position;

                radius = 5f;

                carCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
            }
        }
        if (inOpened)
        {
            mainCamera.gameObject.SetActive(true);

            MoveVechicle();
            VehicleSteering();
            ApplyBreaks();
        }
    }

    void MoveVechicle()
    {
        presentAcceleration = accelerationForce * Input.GetAxis("Vertical");

        foreach (WheelCollider wheel in wheelsColliders)
        {
            wheel.motorTorque = presentAcceleration;
        }
    }

    void VehicleSteering()
    {
        presentTurnAngle = wheelsTorque * Input.GetAxis("Horizontal");
        wheelsColliders[0].steerAngle = presentTurnAngle;
        wheelsColliders[1].steerAngle = presentTurnAngle;

        for (int i = 0; i < wheelsColliders.Length; i++)
        {
            SteeringWheels(wheelsColliders[i], wheelsTransform[i]);
        }
    }

    void SteeringWheels(WheelCollider wC,Transform wT)
    {
        Vector3 positon;
        Quaternion rotation;

        wC.GetWorldPose(out positon, out rotation);
        wT.position = positon;
        wT.rotation = rotation;
    }

    void ApplyBreaks()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            pressentBreakForce = breakingForce;   
        }
        else
        {
            pressentBreakForce = 0f;
        }

        for (int i = 0; i < wheelsColliders.Length; i++)
        {
            wheelsColliders[i].brakeTorque = pressentBreakForce;
        }
    }
}
