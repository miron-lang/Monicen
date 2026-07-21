using StarterAssets;
using UnityEngine;

public class CarNavigator : MonoBehaviour
{
    [Header("Car Info")]
    public float movingSpeed = 17f;
    public float turningSpeed = 150f;
    [SerializeField] float stopSpeed = 0.5f;
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
        if (Physics.Raycast(senser.transform.position, senser.transform.forward, out hitInfo, detectionRange))
        {
            print(hitInfo.transform.name);
            CharacterNavigatorScript characterNpc = hitInfo.transform.GetComponent<CharacterNavigatorScript>();
            ThirdPersonController player = hitInfo.transform.GetComponent<ThirdPersonController>();

            if (characterNpc != null || player != null)
            {
                print("Stop");
                movingSpeed = 0f;
                return;
            }

            else if (characterNpc == null && player == null)
            {
                movingSpeed = 5f;
            }

        }
        Drive();
    }

    public void LoceteDesination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
    }

    public void Drive()
    {
        movingSpeed = 5f;

        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0f;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopSpeed)
            {
                destinationReached = false;
                Quaternion targetRatation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRatation, turningSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movingSpeed * Time.deltaTime);
            }
            else
            {
                destinationReached = true;
            }
        }
    }

}