using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        healthNpc = maxHealthNpc;
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
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
        print("Òû ëîõ");
        player.kills++;
        Object.Destroy(gameObject, 1f);
    }

    public void LoceteDesination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
    }

    public void Walk()
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopSpeed)
            {
                destinationReached = false;
                Quaternion targetRatation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRatation, turningSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * moveingSpeed * Time.deltaTime);
            }
            else
            {
                destinationReached = true;
            }
        }
    }

}