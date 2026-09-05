using Oculus.Interaction.Editor;
using System.Xml.Serialization;
using UnityEngine;

public class Vechicle : MonoBehaviour
{
    [Header("Wheel's and object's")]
    public WheelCollider[] wheelsColliders;
    public Transform[] wheelsTransform;
    public Transform collidersEmpty;
    public Transform transformsEmpty;
    public Transform doorPositon;
    public Misions misionsEmpty;
    public GameObject carCamera;

    [Header("Vechicle steering")]
    public float wheelsTorque;

    [Header("Vechicle engine")]
    public float maxSpeedKmH = 120f;
    public float reverseMaxSpeedKmH = 45f;
    public float additionalAcceleration = 7f;
    public float accelerationForce;
    public float breakingForce;
    public float reverseBreakingForce = 150000f;
    public float driftBreakForce = 15000f;
    [Range(0.05f, 1f)]
    public float driftSidewaysStiffness = 0.55f;
    public float driftTurnForce = 3f;
    public float reverseDeceleration = 35f;

    [Header("Vechical security")]
    public Transform player;
    public bool inOpened = false;
    public float radius = 5f;

    [Header("Disable things")]
    public GameObject mainCamera;

    [Header("Pressent data")]
    private float pressentBreakForce = 0f;
    private float presentTurnAngle = 0f;
    private float presentAcceleration = 0f;
    private Rigidbody rb;
    private float[] normalSidewaysStiffness;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wheelsColliders = collidersEmpty.GetComponentsInChildren<WheelCollider>();
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = maxSpeedKmH / 3.6f;

        normalSidewaysStiffness = new float[wheelsColliders.Length];

        for (int i = 0; i < wheelsColliders.Length; i++)
        {
            normalSidewaysStiffness[i] = wheelsColliders[i].sidewaysFriction.stiffness;
        }

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

                if (misionsEmpty.cuurentMission == 1)
                {
                    misionsEmpty.NextMision();
                    player.gameObject.GetComponent<Player>().currentMoney += 750;
                }
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
            //ApplyBreaks();
        }
    }

    void MoveVechicle()
    {
        //presentAcceleration = accelerationForce * Input.GetAxis("Vertical");

        //foreach (WheelCollider wheel in wheelsColliders)
        //{
        //    wheel.motorTorque = presentAcceleration;
        //}
        float verticalInput = Input.GetAxis("Vertical");
        float currentSpeedkmH = rb.linearVelocity.magnitude * 3.6f;
        float signedSpeedKmH = Vector3.Dot(rb.linearVelocity, transform.forward) * 3.6f;
        bool belowMaximumSpeed = currentSpeedkmH < maxSpeedKmH;
        bool belowReverseMaximumSpeed = signedSpeedKmH > -reverseMaxSpeedKmH;
        bool brakingBeforeReverse = verticalInput < -0.1 && signedSpeedKmH > 1f;
        bool drifting = Input.GetKey(KeyCode.Space);
        float steeringInput = Input.GetAxis("Horizontal");

        if (brakingBeforeReverse)
        {
            presentAcceleration = 0;
        }

        else if (verticalInput > 0f && belowMaximumSpeed)
        {
            presentAcceleration = accelerationForce * verticalInput;
        }

        else if (verticalInput < 0f && belowReverseMaximumSpeed)
        {
            presentAcceleration = accelerationForce * verticalInput;
        }

        else
        {
            presentAcceleration = 0f;
        }

        for (int i = 0;i < wheelsColliders.Length; i++)
        {
            bool rearWheel = i >= 2;

            wheelsColliders[i].motorTorque = !drifting || rearWheel ? presentAcceleration:0f;
        }

        if (verticalInput > 0f && belowMaximumSpeed && !drifting)
        {
            rb.AddForce(transform.forward * additionalAcceleration, ForceMode.Acceleration);
        }

        if (drifting)
        {
            rb.AddTorque(Vector3.up * steeringInput * driftTurnForce, ForceMode.Acceleration);
        }

        if (verticalInput < 0f && belowReverseMaximumSpeed && !brakingBeforeReverse)
        {
            rb.AddForce(-transform.forward * additionalAcceleration, ForceMode.Acceleration);
        }

        if (!belowMaximumSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * (maxSpeedKmH / 3.6f);
        }

        if (!belowReverseMaximumSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * (reverseMaxSpeedKmH / 3.6f);
        }

        if (brakingBeforeReverse)
        {
            Vector3 horizantalVelosity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
            Vector3 verticalVelosoty = rb.linearVelocity - horizantalVelosity;

            horizantalVelosity = Vector3.MoveTowards(horizantalVelosity, Vector3.zero, reverseDeceleration * Time.deltaTime);
            rb.linearVelocity = horizantalVelosity + verticalVelosoty;
        }

        for (int i = 0; i < wheelsColliders.Length; i++)
        {
            bool rearWheel = i >= 2;

            if (brakingBeforeReverse)
            {
                pressentBreakForce = reverseBreakingForce;
            }

            else if (drifting && rearWheel)
            {
                pressentBreakForce = driftBreakForce;
            }

            else
            {
                pressentBreakForce = 0;
            }

            wheelsColliders[i].brakeTorque = pressentBreakForce;
            WheelFrictionCurve sidewaysFriction = wheelsColliders[i].sidewaysFriction;
            sidewaysFriction.stiffness = drifting && rearWheel ? driftSidewaysStiffness : normalSidewaysStiffness[i];
            wheelsColliders[i].sidewaysFriction = sidewaysFriction;
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

    }
}
