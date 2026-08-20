using StarterAssets;
using UnityEngine;

public class CarNavigator : MonoBehaviour
{
    [Header("Car Info")]
    public float movingSpeed = 17f;
    public float maxMovingSped = 20f;
    public float turningSpeed = 150f;
    [SerializeField] float stopSpeed = 1.75f;
    public GameObject senser;
    public float detectionRange = 10f;

    [Header("Destination")]
    public Vector3 destination;
    public bool destinationReached;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        Debug.DrawRay(senser.transform.position, senser.transform.forward * detectionRange, Color.red);
        //if (Physics.Raycast(senser.transform.position, senser.transform.forward, out hitInfo, detectionRange))
        //{
        //    print(hitInfo.transform.name);
        //    CharacterNavigatorScript characterNpc = hitInfo.transform.GetComponentInParent<CharacterNavigatorScript>();
        //    ThirdPersonController player = hitInfo.transform.GetComponentInParent<ThirdPersonController>();
        //    PiliceNavigator pilice = hitInfo.transform.GetComponentInParent<PiliceNavigator>();

        //    if (characterNpc != null || player != null || pilice)
        //    {
        //        print("Stop");
        //        movingSpeed = 0f;
        //        return;
        //    }

        //}

        movingSpeed = maxMovingSped;
        Drive();
    }

    public void LoceteDesination(Vector3 destination)
    {
        destination.y = transform.position.y;
        this.destination = destination;
        destinationReached = false;
    }

    public void Drive()
    {
        //movingSpeed = maxMovingSped;

            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0f;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopSpeed)
            {
                destinationReached = false;
                Quaternion targetRatation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRatation, turningSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, destination, movingSpeed * Time.deltaTime);
            }
            else
            {
                destinationReached = true;
            }
    }

}