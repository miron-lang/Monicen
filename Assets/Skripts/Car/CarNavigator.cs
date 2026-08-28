using UnityEngine;

public class CarNavigator : MonoBehaviour
{
    [Header("Car Info")]
    public float movingSpeed = 17f;
    private float maxMovingSped = 17f;
    public float turningSpeed = 150f;
    [SerializeField] float stopSpeed = 1.75f;
    public GameObject senser;
    public float detectionRange = 10f;
    [SerializeField] float acelerationSpeed = 4f;
    [SerializeField] float turnBrakingSpeed = 60f;
    [SerializeField, Range(0.05f, 1f)] float minimumTurnSpeed = 0.05f;

    [Header("Destination")]
    public Vector3 destination;
    public bool destinationReached;

    [SerializeField] float detectionHalfWidht = 1.7f;
    [SerializeField] float detectionHalfHeight = 0.9f;
    [SerializeField] float obstacleStopDistance = 2f;
    [SerializeField] LayerMask obstacleMask = ~0;
    [SerializeField] float obstacleBreakingSpeed = 80f;
    [SerializeField] float obstacleCheckInterval = 0.1f;
    float nextObstacleChachTime;
    bool cachedObstacleDetected;
    float cachedObstacleSpeed;

† † // Start is called once before the first execution of Update after the MonoBehaviour is created
† † void Start()
    {
        cachedObstacleSpeed = maxMovingSped;

        // —луч€йно€ зудуршка
        nextObstacleChachTime = Time.time + Random.Range(0f, obstacleCheckInterval);
    }

† † // Update is called once per frame
† † void Update()
    {
        // раз в определЄное врем€
        if (Time.time >= nextObstacleChachTime)
        {
            nextObstacleChachTime = Time.time + obstacleCheckInterval;
            cachedObstacleDetected = TryGetObstacleSpeed(out cachedObstacleSpeed);
        }

        Drive();
    }

    public void LoceteDestination(Vector3 destination)
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

            float turnAngle = Vector3.Angle(transform.forward, destinationDirection);
            float turnAlignment = Mathf.InverseLerp(70f, 5f, turnAngle);
            float speedMultiplier = Mathf.Lerp(minimumTurnSpeed, 1f, turnAlignment);
            float targetSpeed =  maxMovingSped * speedMultiplier;

            // «амедление дл€ препетсвтвий
            if (cachedObstacleDetected)
            {
                targetSpeed = Mathf.Min(targetSpeed, cachedObstacleSpeed);
            }

            // если желаемо€ скрость ниже текущ€й смшиа входит в павороти торозит иначе выходит и разгон€еца
            float speedChange = targetSpeed < movingSpeed ? (cachedObstacleDetected ? obstacleBreakingSpeed : turnBrakingSpeed) : acelerationSpeed;

            movingSpeed = Mathf.MoveTowards(movingSpeed, targetSpeed, speedChange * Time.deltaTime);

            Quaternion targetRatation = Quaternion.LookRotation(destinationDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRatation, turningSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, destination, movingSpeed * Time.deltaTime);
        }

        else
        {
            destinationReached = true;
        }
    }

    bool TryGetObstacleSpeed(out float obstacleSpeed)
    {
        obstacleSpeed = maxMovingSped;

        if (senser == null)
        {
            return false;
        }

        Vector3 halfExtents = new Vector3(detectionHalfWidht, detectionHalfHeight, 0.15f);
        Vector3 castOrigin = senser.transform.position + senser.transform.up * detectionHalfHeight;

        bool obstacleDetected = Physics.BoxCast(castOrigin, halfExtents, senser.transform.forward, out RaycastHit hitInfo, senser.transform.rotation, detectionRange, obstacleMask, QueryTriggerInteraction.Ignore);

        if (!obstacleDetected)
        {
            return false;
        }

        float safeSpeedPercent = Mathf.InverseLerp(obstacleStopDistance, detectionRange, hitInfo.distance);

        obstacleSpeed = maxMovingSped * safeSpeedPercent;
        return true;
    }

}