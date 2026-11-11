using UnityEngine;
using System.Collections;   

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

    [Header("Vehicle impact")]
    [SerializeField] float vehiceleRegdolTime = 4f;
    [SerializeField] float vehicleImpactForceMultiplier = 45f;
    [SerializeField] float vehicleUpForce = 3f;
    [SerializeField] bool isKnockedDown; // NPC сечяс лежит регдолом
    [SerializeField] GameObject activeRagdoll; // Созданый регдол
    [SerializeField] Rigidbody activeRagdollHips; // Rigidbody таза регдола - по нему узнаём новое место NPC
    [SerializeField] Renderer[] chatacterRenderers; // Все видимые чясти исходной модели
    [SerializeField] Collider[] characterColliders; // Коллайдеры исходного NPC

    [SerializeField] float obstacleCheckRadiuse = 0.25f;
    [SerializeField] float obstacleCheckDistance = 0.55f;
    [SerializeField] LayerMask obstacelMask = ~0;
    readonly RaycastHit[] obstacleHits = new RaycastHit[8];

    [SerializeField] bool isVoidStarted = false;
    private int timer = 0;

    [SerializeField] float avoidTime = 0.5f;
    bool isAvoding;
    float avoidEndTime;
    Vector3 avoidDirection;
    [SerializeField] WantedPlayer wantedPlayer;

    private void Awake()
    {
        wantedPlayer = GameObject.FindFirstObjectByType<WantedPlayer>();
    }

    void Start()
    {
        waypointNavigator = GetComponent<WaypointNavigator>();
        player = FindAnyObjectByType<Player>();

        healthNpc = maxHealthNpc;

        chatacterRenderers = GetComponentsInChildren<Renderer>(true);
        characterColliders = GetComponentsInChildren<Collider>(true);
    }

    // Вызывается каждый кадр и запускает движение NPC.
    void Update()
    {
        if (dedth || isKnockedDown) return;

        IsEscaping();

        Walk();
    }

    public void NpcGetDamage(float takeDamage, Transform attacker, Vector3 bulletPositon)
    {
        wantedPlayer.firstDamage = true;

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

        if (!isVoidStarted)
        {
            isVoidStarted = true;
            EvryTimeHeted(attacker, bulletPositon);
        }
    }
    void EvryTimeHeted(Transform attacker, Vector3 bulletPositon)
    {
        timer++;

        if (timer >= 10)
        {
            timer = 0;
            isVoidStarted = false;
            return;
        }

        NpcGetDamage(1f, attacker, bulletPositon);
        Invoke("EvryTimeHeted", 1f);
    }

    public void RunAway(Vector3 dangerPosition)
    {
        isEscaping = true;

        anim.SetBool("Run", true);

        escapeEndTime = Time.time + escapeTime;

        if (waypointNavigator == null || !waypointNavigator.StartEscape(dangerPosition))
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

    public void HitByVehicle(float damage, Vector3 vehicleVelocity, Vector3 hitPosition)
    {
        // Не обрабатывоем второй удар, если NPC уже мёртв или уже лежит
        if (dedth || isKnockedDown) return;

        healthNpc -= damage;

        // Урон от удара - создаём регдол на 10 секунд и удоляем NPC
        if (healthNpc <= 0f)
        {
            dedth = true;
            SpawnRagdoll(vehicleVelocity, hitPosition, 10f, vehicleImpactForceMultiplier);
            Destroy(gameObject);
            return;
        }

        // Выжел - времено ложится регдолом, затем возращяется к маошруту
        StartCoroutine(RecoverAfterVehicleImpact(vehicleVelocity, hitPosition));
    }

    IEnumerator RecoverAfterVehicleImpact(Vector3 vehicleVelosity, Vector3 hitPosition)
    {
        isKnockedDown = true;
        isEscaping = false;

        // Остонавливоем внимации исходной модели
        if (anim != null)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            anim.enabled = false;
        }

        // Скрываем живого NPC, что бы оставлся токо регдол
        SetCharacterVisibleAndCollideble(false);

        // Создаём и толкаем регдол
        SpawnRagdoll(vehicleVelosity, hitPosition, 0f, vehicleImpactForceMultiplier);

        // NPC лежит указеное время
        yield return new WaitForSeconds(vehiceleRegdolTime);

        // Переносим NPC туда, куда упал регдол
        if (activeRagdollHips != null)
        {
            transform.position = new Vector3 (activeRagdollHips.position.x, transform.position.y, activeRagdollHips.position.z);
        }

        Destroy(activeRagdoll);

        activeRagdoll = null;
        activeRagdollHips = null;

        // Возоащаем модель и коллайдеры
        SetCharacterVisibleAndCollideble(true);

        if (anim != null)
        {
            anim.enabled = true;
            anim.Rebind();
            anim.Update(0f);
        }

        // После этого обычны Update снова продлжит WayPoint-логику
        isKnockedDown = false;
    }

    void SetCharacterVisibleAndCollideble(bool enabled)
    {
        for (int i = 0; i < chatacterRenderers.Length; i++)
        {
            chatacterRenderers[i].enabled = enabled;
        }

        for (int i = 0; i < characterColliders.Length; i++)
        {
            characterColliders[i].enabled = enabled;
        }
    }


    void SpawnRagdoll(Vector3 impactVelocity, Vector3 hitPosition, float destroyAdterSeconds, float forceMultiper)
    {
        if (regdolGameObject == null)
        {
            return;
        }

        activeRagdoll = Instantiate(regdolGameObject, transform.position, transform.rotation);

        activeRagdollHips = activeRagdoll.GetComponentInChildren<Rigidbody>();

        if (activeRagdollHips != null)
        {
        Vector3 impactDiraction = new Vector3(impactVelocity.x, 0, impactVelocity.z);

            // Запосное наровление, если бибика почти не движица
            if (impactDiraction.sqrMagnitude < 0.01f)
            {
                impactDiraction = transform.forward;
            }

            activeRagdollHips.AddForceAtPosition(impactDiraction.normalized * impactVelocity.magnitude * forceMultiper + Vector3.up * vehicleUpForce, hitPosition, ForceMode.Impulse);
        }

        // Для смерти: удалить регдол через 10 секунд
        if (destroyAdterSeconds > 0f)
        {
            Destroy(activeRagdoll, destroyAdterSeconds);
        }
    }

    Vector3 GetFreeMoveDirection(Vector3 destinationDirection)
    {
        if (isAvoding)
        {
            if (Time.time < avoidEndTime && !IsPathBloked(avoidDirection))
            {
                return avoidDirection;
            }

            isAvoding = false;
        }

        Vector3 freeDiraction = NpcMovementUtility.GetClearDirection(transform, destinationDirection, obstacleCheckRadiuse, obstacleCheckDistance, obstacelMask);

        if (freeDiraction == Vector3.zero)
        {
            return destinationDirection;
        }

        if (Vector3.Dot(freeDiraction, destinationDirection.normalized) < 0.99f)
        {
            isAvoding = true;
            avoidDirection = freeDiraction;
            avoidEndTime = Time.time + avoidTime;
        }
        //}

        //if (!IsPathBloked(destinationDirection))
        //{
        //    return destinationDirection;
        //}

        //Vector3 rightDiraction = Quaternion.Euler(0f, 110f, 0f) * destinationDirection;

        //if (!IsPathBloked(rightDiraction))
        //{
        //    isAvoding = true;
        //    avoidDirection = rightDiraction;
        //    avoidEndTime = Time.time + avoidTime;
        //    return avoidDirection;
        //}

        //Vector3 leftDiraction = Quaternion.Euler(0f, -110f, 0f) * destinationDirection;

        //if (!IsPathBloked(leftDiraction))
        //{
        //    isAvoding = true;
        //avoidDirection = leftDiraction;
        //avoidEndTime = Time.time + avoidTime;
        //    return avoidDirection;

        //return Vector3.zero;

        return freeDiraction;
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

               Vector3 moveDiraction = GetFreeMoveDirection(destinationDirection.normalized);

                if (moveDiraction == Vector3.zero)
                {
                    return;
                }

                Quaternion targetRatation = Quaternion.LookRotation(moveDiraction);

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