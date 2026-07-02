using UnityEngine;

public class CharacterNavigatorScript : MonoBehaviour
{
    [Header("Character Info")]
    public float movingSpeed = 1f;
    public float turningSpeed = 300f;
    [SerializeField] float stopSpeed = 0.3f;

    [Header("Destination")]
    public Vector3 destination;
    public bool destinationReached;

    public float healthNpc = 100;
    public int maxHealthNpc = 100;

    void Start()
    {
        healthNpc = maxHealthNpc;
    }

    void Update()
    {
        Walk();
    }

    public void NpcGetDamage(float takeDamage)
    {
        healthNpc -= takeDamage;
        if (healthNpc <= 0f) Death();
    }

    void Death()
    {
        print("Ты лох");
        Object.Destroy(gameObject, 1f);
    }

    public void LocateDestination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
    }

    public void Walk()
    {
        float distance = Vector3.Distance(transform.position, destination);
        if (distance >= stopSpeed)
        {
            destinationReached = false;
            Vector3 dir = (destination - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
            }
            transform.Translate(Vector3.forward * movingSpeed * Time.deltaTime);
        }
        else
        {
            destinationReached = true;
        }
    }
}