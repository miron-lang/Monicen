using Meta.XR.Simulator.Editor;
using System.Collections;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class MainRifleSkript : MonoBehaviour
{
    public Animator anim;

    public float nextTimeToShoot = 1.5f;
    public float fireCharge = 10f;

    public float shootLengthRange = 40f;

    public GameObject metalEffect;

    [SerializeField] GameObject blood;
    [SerializeField] float rifleDamage = 25f;

    public Transform localOrigin;

    public GameObject player;
    [SerializeField] RuntimeAnimatorController animationController;

    [SerializeField] float animShootDelay = 0.5f;

    public bool isItemInInfertoryHandGunEdition = false;

    [SerializeField] Transform lazerOrigin;

    //public CharacterRetarget characterRetarget;
    Camera cam;
    [SerializeField] GameObject bullet;
    public GameObject lazer;

    bool isShootWaiting;

    [Header("Rife Ammuniton and reloading")]
    public int maxAmmunition = 25;
    public int mag = 10;
    public int presentAmmunition;
    public float reloadingTime = 10.5f;
    public bool setReloading = false;
    public bool isAutomaticFire;
    public float damageDelay = 0.15f;

    [Header("Sounds")]
    [SerializeField] AudioSource shootSound;

    [Header("Sound And Ui")]
    public GameObject ammoOutUi;
    public TMP_Text ammoLeft;
    public TMP_Text magLeft;
    [SerializeField] GameObject ammoMagLeft;

    [Header("Camera")]
    [SerializeField] GameObject aimCam;
    [SerializeField] GameObject thirdPersonCam;

    [Header("RPG explosion")]
    [SerializeField] bool useExplosion = false;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] LayerMask explosionMask = ~0;
    readonly Collider[] explodeAttackResults = new Collider[32];

    [Header("Shotgun")]
    [SerializeField] bool useShootgun = false;
    [SerializeField] int shotgunPelletCount = 10;
    [SerializeField] float shotgunSpreadAngle = 7f;

    private void OnEnable()
    {
        player.transform.GetComponent<Animator>().runtimeAnimatorController = animationController;
        ammoMagLeft.SetActive(true);
        ammoOutUi.SetActive(false);
    }
    void Start()
    {

    }

    void Awake()
    {
        presentAmmunition = maxAmmunition;

        cam = Camera.main;
        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;
    }

    void Update()
    {
        //if (characterRetarget.enabled)
        //{
        //    Lazer();
        //}

        if (setReloading)
        {
            return;
        }

        if (presentAmmunition <= 0 && mag > 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aimCam.SetActive(true);
            thirdPersonCam.SetActive(false);
            anim.SetBool("Aim", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            aimCam.SetActive(false);
            thirdPersonCam.SetActive(true);
            anim.SetBool("Aim", false);
            lazer.SetActive(false);
        }
        if (anim.GetBool("Aim"))
        {
            Lazer();
        }


        if (isAutomaticFire ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time >= nextTimeToShoot)
            {
                anim.SetBool("Shoot", true);
                Invoke("StopShootAnimation", animShootDelay);
                nextTimeToShoot = Time.time + 1f / fireCharge;
                if (!isShootWaiting)
                {
                    isShootWaiting = true;
                    Invoke("Shoot", damageDelay);
                }
                //Shoot();
            }
        }
    }

    IEnumerator ShowAmmoOut()
    {
        ammoOutUi.SetActive(true);
        yield return new WaitForSeconds(2f);
        ammoOutUi.SetActive(false);
    }

    void Lazer()
    {
        RaycastHit hitInfo;
        Ray ray;
        if (anim.GetBool("Aim"))
        {
            ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }
        else
        {
            ray = new Ray(lazerOrigin.position, lazerOrigin.forward);
        }
        if (Physics.Raycast(ray, out hitInfo, shootLengthRange))  // && characterRetarget.enabled)
        {
            //GameObject point = Instantiate(lazer, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            lazer.SetActive(true);
            lazer.transform.position = hitInfo.point;
            lazer.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
            lazer.transform.localScale = Vector3.one * (0.02f + hitInfo.distance * 0.00425f);
        }
        else
        {
            lazer.SetActive(false);
        }
    }
    IEnumerator Reload()
    {
        setReloading = true;
        anim.SetBool("Reload", true);
        string name = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        print(name);
        yield return new WaitForSeconds(reloadingTime);
        anim.SetBool("Reload", false);
        presentAmmunition = maxAmmunition;
        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;
        setReloading = false;
    }

    void Shoot()
    {
        isShootWaiting = false;
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (mag <= 0 && presentAmmunition <= 0)
        {
            StartCoroutine(ShowAmmoOut());
            return;
        }

        presentAmmunition--;

        if (presentAmmunition <= 0 && mag > 0)
        {
            mag--;
        }

        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;

        RaycastHit hitInfo;
        Ray ray;
        if (anim.GetBool("Aim"))
        {
            ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }
        else
        {
            ray = new Ray(lazerOrigin.position, lazerOrigin.forward);
        }

        if (shootSound != null)
            shootSound.Play();

        if (useShootgun)
        {
            ShootShootgun(ray);
            return;
        }

        if (bullet != null)
        {
            GameObject newBullet = Instantiate(bullet, localOrigin.position, localOrigin.rotation);
            newBullet.transform.forward = ray.direction;
        }

        if (Physics.Raycast(ray, out hitInfo, shootLengthRange))
        {
            if (useExplosion)
            {
                ProcessExplode(hitInfo);
            }
            else
            {
                DamageTareget(hitInfo.transform, hitInfo.point, hitInfo.normal);
            }

        }
    }

    void ProcessExplode(RaycastHit hitInfo)
    {
        if (metalEffect != null)
        {
            GameObject createMetalEffect = Instantiate(metalEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        }

        int hitCount = Physics.OverlapSphereNonAlloc(hitInfo.point, explosionRadius, explodeAttackResults, explosionMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            if (explodeAttackResults[i] == null)
            {
                continue;
            }

            DamageTareget(explodeAttackResults[i].transform, explodeAttackResults[i].transform.position, (explodeAttackResults[i].transform.position - hitInfo.point).normalized);
        }
    }

    void DamageTareget(Transform hitTransofrm, Vector3 hitPoint, Vector3 hitNormal)
    {
        PoliceOfficer police = hitTransofrm.GetComponent<PoliceOfficer>();
        CharacterNavigatorScript01 npc = hitTransofrm.GetComponent<CharacterNavigatorScript01>();
        SimpleGuncsterSkript guncter = hitTransofrm.GetComponent<SimpleGuncsterSkript>();
        Boss boss = hitTransofrm.GetComponent<Boss>();

        if (police != null)
        {
            GameObject createBloodEffect = Instantiate(blood, hitPoint, Quaternion.LookRotation(hitNormal));
            police.PoliceGetDamage(rifleDamage);
        }

        else if (npc != null)
        {
            GameObject createBloodEffect = Instantiate(blood, hitPoint, Quaternion.LookRotation(hitNormal));
            npc.NpcGetDamage(rifleDamage, player.transform, hitPoint);
        }

        else if (guncter != null)
        {
            GameObject createBloodEffect = Instantiate(blood, hitPoint, Quaternion.LookRotation(hitNormal));
            guncter.GuncsterGetDamage(rifleDamage);
        }

        else if (boss != null)
        {
            GameObject createBloodEffect = Instantiate(blood, hitPoint, Quaternion.LookRotation(hitNormal));
            boss.BossGetDamage(rifleDamage);
        }
        else if (!useExplosion)
        {
            GameObject createMetalEffect = Instantiate(metalEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        }
    }

    void ShootShootgun(Ray ray)
    {
        for (int i = 0; i < shotgunPelletCount; i++)
        {
            Vector2 spread = Random.insideUnitCircle * shotgunSpreadAngle;
            Vector3 pelletDirection = Quaternion.Euler(spread.y, spread.x, 0f) * ray.direction;

            if (Physics.Raycast(ray.origin, pelletDirection, out RaycastHit pelletHit, shootLengthRange))
            {
                DamageTareget(pelletHit.transform, pelletHit.point, pelletHit.normal);
            }
        }
    }

    void StopShootAnimation()
    {
        anim.SetBool("Shoot", false);
    }
}