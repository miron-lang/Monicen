using Meta.Voice.Net.WebSockets;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class CharacterNavigatorScript : MonoBehaviour
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

    private NavMeshAgent navMeshAgent;
    private bool useNavMesh;
    private Rigidbody rb;

    [SerializeField] float obcstacleCheckRadius = 0.2f;
    [SerializeField] float obcstacleCheckDisctance = 1.2f;
    [SerializeField] LayerMask obstacleMask = ~0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<Player>();
        healthNpc = maxHealthNpc;
        navMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshHit hit  = default;
        useNavMesh = navMeshAgent != null && NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas);
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = useNavMesh;
            if (useNavMesh)
            {
                navMeshAgent.Warp(hit.position);
                navMeshAgent.speed = moveingSpeed;
                navMeshAgent.angularSpeed = turningSpeed;
                navMeshAgent.stoppingDistance = stopSpeed;

                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (useNavMesh)
        {
            Walk();
        }
    }

    void FixedUpdate()
    {
        if (!useNavMesh)
        {
            Walk();
        }
    }

    public void NpcGetDamage(float takeDamage)
    {
        healthNpc -= takeDamage;

        if (healthNpc <= 0f && !dedth)
        {
            Death();
        }
    }

    void Death()
    {
        dedth = true;
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
        }
        if (player != null)
        {
            player.kills++;
        }
        Object.Destroy(gameObject, 1f);
    }

    public void LoceteDesination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;

        if (useNavMesh && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(destination);
        }
    }

    public void Walk()
    {
        if (dedth)
        {
            return;
        }

        if (useNavMesh && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            destinationReached = !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
            return;
        }

        Vector3 destinationDirection = destination - transform.position;
        destinationDirection.y = 0f;

        float destinationDistance = destinationDirection.magnitude;


        if (destinationDistance >= stopSpeed)
        {
            destinationReached = false;
            Vector3 moveDiraction = NpcMovementUtility.GetClearDirection(transform, destinationDirection, obcstacleCheckRadius, obcstacleCheckDisctance, obstacleMask);
            if (moveDiraction == Vector3.zero)
            {
                StopRb();
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(moveDiraction);
            Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningSpeed * Time.fixedDeltaTime);

            if (rb != null)
            {
                rb.MoveRotation(nextRotation);
                Vector3 velocity = nextRotation * Vector3.forward * moveingSpeed;
                rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
            }
            else
            {
                transform.rotation = nextRotation;
                transform.position += transform.forward * moveingSpeed * Time.fixedDeltaTime;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * moveingSpeed * Time.deltaTime);
        }
        else
        {
            destinationReached = true;
            StopRb();
        }

    }

    public void StopRb()
    {
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

}