using UnityEngine;
using UnityEngine.UIElements;

// Старая версия навигации NPC до добавления NavMesh.
// ВАЖНО: не кладите этот файл в Assets одновременно с текущим
// CharacterNavigatorScript.cs, потому что имена классов совпадают.
public class CharacterNavigatorScript01 : MonoBehaviour
{
    [Header("Character Info")]
    public float moveingSpeed = 1f;
    public float turningSpeed = 300f;
    [SerializeField] float stopSpeed = 0.3f;

    [Header("Destination")]
    public Vector3 destination;
    public bool destinationReached;

    private bool dedth = false;

    public float healthNpc = 100;
    public int maxHealthNpc = 100;

    private Player player;
    private WaypointNavigator waypointNavigator;

    [SerializeField] float escapeDistance = 12f;
    [SerializeField] float escapeTime = 5f;
    [SerializeField] float escapeSpeed = 5f;
    Vector3 escapeDesetination;
    bool isEscaping;
    float escapeEndTime;

    [SerializeField] Animator anim;
    [SerializeField] GameObject regdolGameObject;

    [SerializeField] float obstacleCheckRadiuse = 0.25f;
    [SerializeField] float obstacleCheckDistance = 0.55f;
    [SerializeField] LayerMask obstacelMask = ~0;
    readonly RaycastHit[] obstacleHits = new RaycastHit[8];

    void Start()
    {
        waypointNavigator = GetComponent<WaypointNavigator>();
        player = FindAnyObjectByType<Player>();
        healthNpc = maxHealthNpc;
    }

    // Вызывается каждый кадр и запускает движение NPC.
    void Update()
    {
        IsEscaping();

        Walk();
    }

    public void NpcGetDamage(float takeDamage, Transform attacker, Vector3 bulletPositon)
    {
        healthNpc -= takeDamage;

        if (healthNpc <= 0f && !dedth)
        {
            Death(attacker.position, bulletPositon);
            return;
        }

        if (!dedth && attacker != null)
        {
            print("БЕГ НЕ СРАБОТАЛ");
            RunAway(attacker.position);
        }
    }

    public void RunAway(Vector3 dangerPosition)
    {
        isEscaping = true;

        anim.SetBool("Run", true);

        escapeEndTime = Time.time + escapeTime;

        if (waypointNavigator == null || waypointNavigator.StartEscape(dangerPosition))
        {
            isEscaping = false;
            anim.SetBool("Run", false);
        }

        //Vector3 escapeDiraction = transform.position - dangerPosition;
        //escapeDiraction.y = 0f;

        //if (escapeDiraction.sqrMagnitude < 0.1f)
        //{
        //    escapeDiraction = Random.insideUnitSphere;
        //    escapeDiraction.y = 0;
        //}
        //escapeDiraction.Normalize();
        //escapeDesetination = transform.position + escapeDiraction * escapeDistance;
        //LoceteDestination(escapeDesetination);
    }

    bool IsPathBloked(Vector3 moveDiraction)
    {
        Vector3 castOrigin = transform.position + Vector3.up * Mathf.Max(obstacleCheckRadiuse, 0.25f);
        int hitCount = Physics.SphereCastNonAlloc(castOrigin, obstacleCheckRadiuse, moveDiraction, obstacleHits, obstacleCheckDistance, obstacelMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            Transform hitTransform = obstacleHits[i].transform;

            if (hitTransform != transform && !hitTransform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;

    }

    public bool IsEscaping()
    {   
        if (isEscaping && Time.time >= escapeEndTime)
        {
            isEscaping = false;
            anim.SetBool("Run", false);
        }

        return isEscaping;
    }

    void Death(Vector3 dangerPositon, Vector3 bulletPosition)
    {
        dedth = true;
        print("NPC погиб");

        if (player != null)
        {
            player.kills++;
        }

        Object.Destroy(gameObject);

        if (regdolGameObject != null)
        {
            GameObject obj = Instantiate(regdolGameObject, transform.position, transform.rotation);

            Rigidbody hips = obj.GetComponentInChildren<Rigidbody>();

            if (hips != null && dangerPositon != null)
            {
                Vector3 forceDiraction = transform.position - dangerPositon;
                forceDiraction.y = 0f;
                forceDiraction.Normalize();
                hips.AddForceAtPosition(forceDiraction * 60f + Vector3.up * 7.5f, bulletPosition, ForceMode.Impulse);
                hips.AddTorque(Random.insideUnitSphere * 25f, ForceMode.Impulse);
            }

            Object.Destroy(obj, 10f);
        }
    }

    public void LoceteDestination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
    }

    public void Walk()
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;

            destinationDirection.y = 0f;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopSpeed)
            {
                destinationReached = false;

                if (IsPathBloked(destinationDirection.normalized))
                {
                    return;
                }

                Quaternion targetRatation = Quaternion.LookRotation(destinationDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRatation,
                    turningSpeed * Time.deltaTime
                );

                float currentSpeed = isEscaping ? escapeSpeed : moveingSpeed;

                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            }
            else
            {
                destinationReached = true;
            }
        }
    }
}
