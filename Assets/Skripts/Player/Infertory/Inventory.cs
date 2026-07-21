using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Item Slot's")]
    public GameObject[] weaponsPictures;
    public bool[] isWeaponPicked;
    public bool[] isWeaponActive;
    
    [Header("Waepons to use")]
    public GameObject[] Guns;
    public GameObject lHG;
    public GameObject lHGWAS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < isWeaponPicked.Length; i++)
        {
            if (isWeaponPicked[i])
                weaponsPictures[i].SetActive(true);
            else
                weaponsPictures[i].SetActive(false);
        }
    }

    public void IsRifleActive(int activeWeaponNumber)
    {
        if (activeWeaponNumber < 6 && isWeaponPicked[activeWeaponNumber])
        {
        for (int i = 0; i < isWeaponActive.Length; i++)
        {
            isWeaponActive[i] = false;
        }

            isWeaponActive[activeWeaponNumber] = true;

        for (int i = 0; i < Guns.Length; i++)
        {
            if (isWeaponActive[i])
            {
                Guns[i].SetActive(true);
            }
            else
            {
                Guns[i].SetActive(false);
            }
        }
        ActiveLeftWeapon();
        }
    }

    void ActiveLeftWeapon()
    {
        if (isWeaponActive[0])
        {
            lHG.SetActive(true);
            lHGWAS.SetActive(false);
        }
        else if (isWeaponActive[1])
        {
            lHGWAS.SetActive(true);
            lHG.SetActive(false);
        }
        else
        {
            lHGWAS.SetActive(false);
            lHG.SetActive(false);
        }
    }

}
