using UnityEngine;

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

    // Вызывается один раз при запуске NPC.
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        healthNpc = maxHealthNpc;
    }

    // Вызывается каждый кадр и запускает движение NPC.
    void Update()
    {
        Walk();
    }

    public void NpcGetDamage(float takeDamage)// Transform attacker
    {
        healthNpc -= takeDamage;

        if (healthNpc <= 0f && !dedth)
        {
            Death();
        }
    }

    public void RunAway(Vector3 dangerPosition)
    {

    }

    void Death()
    {
        dedth = true;
        print("NPC погиб");

        if (player != null)
            player.kills++;

        Object.Destroy(gameObject, 1f);
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

                Quaternion targetRatation = Quaternion.LookRotation(destinationDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRatation,
                    turningSpeed * Time.deltaTime
                );

                transform.Translate(
                    Vector3.forward * moveingSpeed * Time.deltaTime
                );
            }
            else
            {
                destinationReached = true;
            }
        }
    }
}
