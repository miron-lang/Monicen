using System.Data;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using System.Collections;

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

    [Header("Cool things")]
    [SerializeField] RectTransform helpUi;
    [SerializeField] TMP_Text helpUiText;
    [SerializeField] float uiMoveTime = 0.3f;
    private bool uiShow;

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

        if (Vector3.Distance(player.transform.position, transform.position) <= distence)
        {
            if (!uiShow)
            {
                uiShow = true;
                helpUi.gameObject.SetActive(true);
                helpUiText.text = "Press: H";
                StartCoroutine(SmoothHelpingUi(true));
            }
            if (Input.GetKeyDown(KeyCode.H))
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
        }
        else if (uiShow)
        {
            uiShow = false;
            StartCoroutine(SmoothHelpingUi(false));
        }

        if (isYouInShop)
        {
            helpUiText.text = "Press: E";

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
                    Invoke("DisableUI", 3f);
                }
                else if (player.currentMoney <= currentGunShop.GetComponent<Gun>().itemPrice && whyNoBuying != null)
                {
                    whyNoBuying.gameObject.SetActive(true);
                    whyNoBuying.text = "Not enough money";
                    Invoke("DisableUI", 3f);
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

    IEnumerator SmoothHelpingUi(bool show)
    {
        float targetStartXPosition = helpUi.anchoredPosition.x;
        float targetXposition = show ? -250 : 250;
        float time = 0f;

        while (time < uiMoveTime)
        {
            time += Time.deltaTime;

            float progress = time / uiMoveTime;
            float newx = Mathf.Lerp(targetStartXPosition, targetXposition, progress);

            helpUi.anchoredPosition = new Vector2(newx, helpUi.anchoredPosition.y);

            yield return null;
        }

        helpUi.anchoredPosition = new Vector2(targetXposition, helpUi.anchoredPosition.y);
    }

    public void DisableUI()
    {
        whyNoBuying.gameObject.SetActive(false);
    }
}

