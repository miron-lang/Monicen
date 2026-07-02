using Meta.XR.Movement.Retargeting;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Rifle : MonoBehaviour
{

    public Animator anim;

    private float nextTimeToShoot = 1.5f;
    public float fireCharge = 10f;

    public float rifleDamage = 25f;

    public float shootLengthRange = 40f;

    public GameObject metalEffect;
    public GameObject lazer;

    public Transform localOrigin;

    [Header("Rife Ammuniton and reloading")]
    public int maxAmmunition = 25;
    public int mag = 10;
    public int presentAmmunition;
    public float reloadingTime = 10.5f;
    public bool setReloading = false;

    public ParticleSystem shootEfect;

    public bool isItemInInfertory = false;

    [Header("Sound And Ui")]
    public GameObject ammoOutUi;
    public TMP_Text ammoLeft;
    public TMP_Text magLeft;

    public GameObject blood;

    public CharacterRetargeter characterRetarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        ammoOutUi.SetActive(false);
        presentAmmunition = maxAmmunition;

        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;
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

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))//Input.GetButton("Fire1") || 
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
        if (Physics.Raycast(localOrigin.position, localOrigin.forward, out hitInfo, shootLengthRange) && characterRetarget.enabled)
        {
            //GameObject point = Instantiate(lazer, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            lazer.SetActive(true);
            lazer.transform.position = hitInfo.point;
            lazer.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
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

        presentAmmunition --;
        shootEfect.Play();

        if (presentAmmunition <= 0 && mag > 0)
        {
            mag--;
        }

        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;

        print("Shoot");
        RaycastHit hitInfo;
        if (Physics.Raycast(localOrigin.position, localOrigin.forward, out hitInfo, shootLengthRange))
        {
            PiliceNavigator police = hitInfo.transform.GetComponent<PiliceNavigator>();
            CharacterNavigatorScript npc = hitInfo.transform.GetComponent<CharacterNavigatorScript>();
            if (state.IsName("ShootPrimaryAssaultRifle"))
            {
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
                    GameObject createMetalEffect = Instantiate(metalEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

            }
        }
    }

}
