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
            isYouInShop = !isYouInShop;
        }

        if (isYouInShop)
        {
            playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentGunShop.tag == "HandGunPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    inventory.isWeaponPicked[0] = true;
                }
                else if (currentGunShop.tag == "HandGunWhithAMuferPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    inventory.isWeaponPicked[1] = true;
                }
                else if (currentGunShop.tag == "UziPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    inventory.isWeaponPicked[2] = true;
                }
                else if (currentGunShop.tag == "ShootGunPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    inventory.isWeaponPicked[3] = true;
                }
                else if (currentGunShop.tag == "BazukaPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    inventory.isWeaponPicked[4] = true;
                }
                else if (currentGunShop.tag == "BayoutPickup" && player.currentMoney >= currentGunShop.GetComponent<Gun>().itemPrice)
                {
                    player.currentMoney -= currentGunShop.GetComponent<Gun>().itemPrice;
                    inventory.isWeaponPicked[5] = true;
                }
            }

            if (Input.GetKey(KeyCode.S) && currentGunShop.GetComponent<Gun>().downGun != null)
            {
                currentGunShop = currentGunShop.GetComponent<Gun>().downGun;
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKey(KeyCode.W) && currentGunShop.GetComponent<Gun>().upGun != null)
            {
                currentGunShop = currentGunShop.GetComponent<Gun>().upGun;
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKey(KeyCode.D) && currentGunShop.GetComponent<Gun>().rightGun != null)
            {
                currentGunShop = currentGunShop.GetComponent<Gun>().rightGun;
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }

            if (Input.GetKey(KeyCode.A) && currentGunShop.GetComponent<Gun>().leftGun != null)
            {
                currentGunShop = currentGunShop.GetComponent<Gun>().leftGun;
                playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentGunShop.GetComponent<Gun>().cameraPosition.transform;
            }
        }
    }
}
