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

    public Transform localOrigin;

    public bool isItemInInfertoryHandGunEdition = false;

    [Header("Rife Ammuniton and reloading")]
    public int maxAmmunition = 25;
    public int mag = 10;
    public int presentAmmunition;
    public float reloadingTime = 10.5f;
    public bool setReloading = false;

    public ParticleSystem shootEfect;

    [Header("Sound And Ui")]
    public GameObject ammoOutUi;
    public TMP_Text ammoLeft;
    public TMP_Text magLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Awake()
    {
        ammoLeft.text = "Ammo Left:" + presentAmmunition;
        magLeft.text = "Mag Left:" + mag;

        ammoOutUi.SetActive(false);
        presentAmmunition = maxAmmunition;
    }


    // Update is called once per frame
    void Update()
    {
        if (setReloading)
        {
            return;
        }

        if (presentAmmunition <= 0 && mag > 0)
        {
            StartCoroutine(Reload());
            return;
        }


        if (Input.GetButtonDown("Fire1"))
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

        print("Shoot");
        RaycastHit hitInfo;
        if (Physics.Raycast(localOrigin.position, localOrigin.forward, out hitInfo, shootLengthRange))
        {
            if (state.IsName("ShootPrimary2GunsAim"))
            {
                GameObject createMetalEffect = Instantiate(metalEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                print(hitInfo.transform.name);
            }
        }
    }

}
