using Meta.XR.Movement.Retargeting;
using System.Collections;
using TMPro;
using UnityEngine;

public class HandGunRifle : MonoBehaviour
{

    public Animator anim;

    private float nextTimeToShoot = 1.5f;
    public float fireCharge = 10f;

    public float shootLengthRange = 40f;

    public GameObject metalEffect;

    [SerializeField] GameObject blood;
    [SerializeField] float rifleDamage = 25f;

    public Transform localOrigin;

    public GameObject player;
    [SerializeField] RuntimeAnimatorController animationController;

    public bool isItemInInfertoryHandGunEdition = false;

    [Header("Rife Ammuniton and reloading")]
    public int maxAmmunition = 25;
    public int mag = 10;
    public int presentAmmunition;
    public float reloadingTime = 10.5f;
    public bool setReloading = false;

    public ParticleSystem shootEfect;

    [SerializeField] Transform lazerOrigin;

    [SerializeField] GameObject ammoMagLeft;

    [Header("Sound And Ui")]
    public GameObject ammoOutUi;
    public TMP_Text ammoLeft;
    public TMP_Text magLeft;

    [Header("Camera")]
    [SerializeField] GameObject aimCam;
    [SerializeField] GameObject thirdPersonCam;

    public CharacterRetargeter characterRetarget;
    Camera cam;
    [SerializeField] GameObject bullet;
    public GameObject lazer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        player.transform.GetComponent<Animator>().runtimeAnimatorController = animationController;
        print("сменили анимацыю");
        ammoMagLeft.SetActive(true);
        ammoOutUi.SetActive(false);
    }
    void Start()
    {

    }

    void Awake()
    {
        cam = Camera.main;
        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;

        presentAmmunition = maxAmmunition;
    }


    // Update is called once per frame
    void Update()
    {
        if (characterRetarget.enabled)
        {
            Lazer();
        }

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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time >= nextTimeToShoot)
            {
                anim.SetBool("Shoot", true);
                nextTimeToShoot = Time.time + 1f / fireCharge;
                //Invoke("Shoot", 0.3f);
                Shoot();
            }
        }
        else
        {
            anim.SetBool("Shoot", false);
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
        print("Reloading...");
        anim.SetBool("Reload", true);
        yield return new WaitForSeconds(reloadingTime);
        anim.SetBool("Reload", false);
        presentAmmunition = maxAmmunition;
        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;
        print("Reloading Finish");
        setReloading = false;
    }

    void Shoot()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (mag <= 0 && presentAmmunition <= 0)
        {
            StartCoroutine(ShowAmmoOut());
            return;
        }

        presentAmmunition--;
        shootEfect.Play();

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
        GameObject newBullet = Instantiate(bullet, localOrigin.position, localOrigin.rotation);
        newBullet.GetComponent<Bullet>().target = ray.direction;
        if (Physics.Raycast(ray, out hitInfo, shootLengthRange))
        {
            PiliceNavigator police = hitInfo.transform.GetComponent<PiliceNavigator>();
            CharacterNavigatorScript npc = hitInfo.transform.GetComponent<CharacterNavigatorScript>();
            print("SOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOT");
            //if (state.IsName("ShootPrimary2GunsAim"))
            {
                print("anim sghooot ");
                if (police != null)
                {
                    GameObject createBloodEffect = Instantiate(blood, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    hitInfo.transform.GetComponent<PoliceOfficer>().PoliceGetDamage(rifleDamage);
                }

                if (npc != null)
                {
                    GameObject createBloodEffect = Instantiate(blood, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    hitInfo.transform.GetComponent<CharacterNavigatorScript>().NpcGetDamage(rifleDamage);
                }

                else
                {
                    print("Effect");
                    GameObject createMetalEffect = Instantiate(metalEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

            }
        }
    }
}
    


