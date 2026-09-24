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
                if (currentGunShop.tag == "HandGunPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }

                    if (inventory.isWeaponPicked[0])
                    {
                        inventory.isWeaponPicked[0] = true;
                        player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    }
                }
                else if (currentGunShop.tag == "HandGunWhithAMuferPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }
                    if (inventory.isWeaponPicked[1])
                    {
                        inventory.isWeaponPicked[1] = true;
                        player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    }
                }
                else if (currentGunShop.tag == "UziPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }

                    if (inventory.isWeaponPicked[2])
                    {
                        inventory.isWeaponPicked[2] = true;
                        player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    }
                }
                else if (currentGunShop.tag == "ShootGunPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }

                    if (inventory.isWeaponPicked[3])
                    {
                        inventory.isWeaponPicked[3] = true;
                        player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    }
                }
                else if (currentGunShop.tag == "BazukaPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }

                    if (inventory.isWeaponPicked[4])
                    {
                        inventory.isWeaponPicked[4] = true;
                        player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    }
                }
                else if (currentGunShop.tag == "BayoutPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    if (misionsEmpty.cuurentMission == 3)
                    {
                        misionsEmpty.NextMision();
                    }

                    if (inventory.isWeaponPicked[5])
                    {
                        inventory.isWeaponPicked[5] = true;
                        player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.S) && currentGunShop.GetComponent<Gun>().downGun != null)
        {
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
            currentGunShop = currentGunShop.GetComponent<Gun>().downGun;
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
        }

        if (Input.GetKey(KeyCode.W) && currentGunShop.GetComponent<Gun>().upGun != null)
        {
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
            currentGunShop = currentGunShop.GetComponent<Gun>().upGun;
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
        }

        if (Input.GetKey(KeyCode.D) && currentGunShop.GetComponent<Gun>().rightGun != null)
        {
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
            currentGunShop = currentGunShop.GetComponent<Gun>().rightGun;
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
        }

        if (Input.GetKey(KeyCode.A) && currentGunShop.GetComponent<Gun>().leftGun != null)
        {
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(false);
            currentGunShop = currentGunShop.GetComponent<Gun>().leftGun;
            currentGunShop.GetComponent<Gun>().gunUi.SetActive(true);
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
        }
    }
}

