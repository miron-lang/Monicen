using System.Data;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Inventory inventory;

    public GameObject currentGunShop;

    public bool isYouInShop = false;

    [Header("Player Info")]
    public GameObject normalCameraPositon;
    public GameObject playerFollowCamera;
    public Player player;
    public float distence = 3f;

    public Misions misionsEmpty;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    public TMP_Text whyNoBuying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinemachineVirtualCamera = playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.Guns.Count() != inventory.isWeaponActive.Count() && inventory.Guns.Count() != inventory.isWeaponPicked.Count() && inventory.Guns.Count() != inventory.weaponsPictures.Count())
        {
            Debug.LogWarning("Колчисто спимкв оружий не совподает");
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= distence && !isYouInShop)
            {
                if (isYouInShop)
                {
                    isYouInShop = false;
                    cinemachineVirtualCamera.Follow = normalCameraPositon.transform;
                }
                else
                {
                    isYouInShop = true;
                    cinemachineVirtualCamera.Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
                }
            }
            else 
            { 
                isYouInShop = false;
                cinemachineVirtualCamera.Follow = normalCameraPositon.transform;
            }
        }

        if (isYouInShop)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inventory.isWeaponPicked[currentGunShop.GetComponent<Gun>().weaponIndex] && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    inventory.isWeaponPicked[currentGunShop.GetComponent<Gun>().weaponIndex] = true;
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;

                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }
                }
                else if (inventory.isWeaponPicked[currentGunShop.GetComponent<Gun>().weaponIndex] && whyNoBuying != null)
                {
                    whyNoBuying.gameObject.SetActive(true);
                    whyNoBuying.text = "You already own this weapon.";
                    Invoke("DisableUI", 1f);
                }
                else if (player.currentMoney <= currentGunShop.GetComponent<Gun>().itemPrice && whyNoBuying != null)
                {
                    whyNoBuying.gameObject.SetActive(true);
                    whyNoBuying.text = "Not enough money";
                    Invoke("DisableUI", 1f);
                }
            }

            if (Input.GetKeyDown(KeyCode.S) && currentGunShop.GetComponent<Gun>().downGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().downGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                cinemachineVirtualCamera.Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKeyDown(KeyCode.W) && currentGunShop.GetComponent<Gun>().upGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().upGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                cinemachineVirtualCamera.Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKeyDown(KeyCode.D) && currentGunShop.GetComponent<Gun>().rightGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().rightGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                cinemachineVirtualCamera.Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKeyDown(KeyCode.A) && currentGunShop.GetComponent<Gun>().leftGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().leftGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                cinemachineVirtualCamera.Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }
        }

    }

    public void DisableUI()
    {
        whyNoBuying.gameObject.SetActive(false);
    }
}

