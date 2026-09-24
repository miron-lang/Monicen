using System.Data;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isYouInShop)
        {
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = normalCameraPositon.transform;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= distence && !isYouInShop)
            {
                isYouInShop = !isYouInShop;
            }
            else 
            { 
                isYouInShop = false;
            }
        }

        if (isYouInShop)
        {
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inventory.isWeaponPicked[currentGunShop.GetComponent<Gun>().weaponIndex] && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    inventory.isWeaponPicked[currentGunShop.GetComponent<Gun>().weaponIndex] = true;
                    player.currentMoney = currentGunShop.GetComponent<Gun>().itemPrice;

                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.S) && currentGunShop.GetComponent<Gun>().downGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().downGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKeyDown(KeyCode.W) && currentGunShop.GetComponent<Gun>().upGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().upGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKeyDown(KeyCode.D) && currentGunShop.GetComponent<Gun>().rightGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().rightGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKeyDown(KeyCode.A) && currentGunShop.GetComponent<Gun>().leftGun != null)
            {
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
                currentGunShop = currentGunShop.GetComponent<Gun>().leftGun;
                if (currentGunShop.GetComponent<Gun>().gunUi != null) currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }
        }

    }
}

