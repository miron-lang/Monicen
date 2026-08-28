using System;
using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] float walkingSpeed;
    [SerializeField] float runingSpeed;
    [SerializeField] float turningSpeed;
    [SerializeField] float stopSpeed = 0.3f;

    [Header("Deatatination")]
    public Vector3 destination;
    public bool destinationReached = false;

    [SerializeField] float healthPolice = 100;
    [SerializeField] int maxHealthPolice = 100;
    [SerializeField] float armorPolice = 0;

    [SerializeField] bool isVoidStarted = false;

    [Range(0, 4)]
    [SerializeField] int startStars;

    [Header("Police Ai")]
    private GameObject playerBody;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float visionRadius;
    [SerializeField] float shootingRadius;
    [SerializeField] bool playerInVisionRadius;
    [SerializeField] bool playerInShootingRedius;

    [Header("Police Shooting")]
    [SerializeField] WantedPlayer wantedPlayer;
    [SerializeField] GameObject shootingRaycastArea;
    [SerializeField] Player player;
    [SerializeField] float timeBtwShoot;
    public float giveDamage = 25f;
    [SerializeField] GameObject oilBlood;

    bool previuseShoot = false;

    [SerializeField] Animator anim;

    private float currentMovingSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthPolice = maxHealthPolice;
        playerBody = GameObject.Find("PlayerArmature");
        wantedPlayer = GameObject.FindFirstObjectByType<WantedPlayer>();
        currentMovingSpeed = walkingSpeed;
        player = GameObject.FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerInShootingRedius = Physics.CheckSphere(transform.position, shootingRadius, playerLayer);

        if (!playerInVisionRadius && !wantedPlayer.wantedLevel[startStars] && !playerInShootingRedius)
        {
            Walk();
        }

        else if (playerInVisionRadius && wantedPlayer.wantedLevel[startStars] && !playerInShootingRedius)
        {
            ChasePlayer();
        }

        if (playerInShootingRedius && wantedPlayer.wantedLevel[startStars])
        {
            ShootAtThePlayer();
        }

    }
    public void LoceteDestination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
    }

    void ChasePlayer()
    {
        //двигаем полицейского в перёд и что бы он сматрел на игрока
        transform.LookAt(playerBody.transform);
        transform.Translate(Vector3.forward * currentMovingSpeed * Time.deltaTime);
        anim.SetBool("Walk", false);
        anim.SetBool("Shoot", false);
        anim.SetBool("Run", true);
        currentMovingSpeed = runingSpeed;
    }

    void ShootAtThePlayer()
    {
        currentMovingSpeed = 0f;
            transform.LookAt(playerBody.transform);
        if (!previuseShoot)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Shoot", true);
            anim.SetBool("Run", false);
            RaycastHit hitInfo;
            if (Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward, out hitInfo, shootingRadius))
            {
                print(hitInfo.transform.name);
                Player playerBody = hitInfo.transform.GetComponent<Player>();
                if (playerBody != null)
                {
                    GameObject createOilBloodEffect = Instantiate(oilBlood, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    player.PlayerGetDamage(giveDamage);
                    print("хп = " + player.health);
                }
            }
            previuseShoot = true;
            Invoke("ActiveShooting", timeBtwShoot);
        }
    }

    void ActiveShooting()
    {
        previuseShoot = false;
    }

    void Walk()
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
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRatation, turningSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * walkingSpeed * Time.deltaTime);
                anim.SetBool("Walk", true);
                anim.SetBool("Shoot", false);
                anim.SetBool("Run", false);
            }

            else
            {
                destinationReached = true;
            }
        }
    }

    public void PoliceGetDamage(float takeDamage)
    {
        wantedPlayer.firstDamage = true;

        if (!isVoidStarted)
        {
            isVoidStarted = true;
            EvryTimeHeted();
        }

        if (armorPolice <= 0)
        {
            if (armorPolice <= 0)
                healthPolice -= takeDamage;
        }

        else if (armorPolice >= 1)
        {
            armorPolice -= takeDamage;
            healthPolice -= takeDamage % 4;
        }

        if (healthPolice <= 0f)
        {
            Death();
        }
    }

    void Death()
    {
        currentMovingSpeed = 0f;
        shootingRadius = 0f;
        player.kills++;
        anim.SetBool("Walk", false);
        anim.SetBool("Shoot", false);
        anim.SetBool("Run", false);
        anim.SetBool("Death", true);
        Destroy(gameObject, 1f);
    }

    void EvryTimeHeted()
    {
        PoliceGetDamage(1f);
        Invoke("EvryTimeHeted", 1f);
    }

}
