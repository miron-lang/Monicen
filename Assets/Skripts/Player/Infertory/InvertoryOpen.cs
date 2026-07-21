using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class InvertoryOpen : MonoBehaviour
{

    public GameObject inventoryUI;
    bool isPause = false;

    [SerializeField] RuntimeAnimatorController animationController;

    public int selectedSlot = 6;

    Inventory inventory;

    [SerializeField] GameObject ammoOutUi;
    [SerializeField] GameObject ammoMagLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = inventoryUI.GetComponent<Inventory>();
        transform.GetComponent<Animator>().runtimeAnimatorController = animationController;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = animationController;
            for (int i = 0; i < inventory.isWeaponActive.Length; i++)
            {
                inventory.isWeaponActive[i] = false;
                inventory.Guns[i].SetActive(false);
            }
                inventory.lHG.SetActive(false);
                inventory.lHGWAS.SetActive(false);
            ammoMagLeft.SetActive(false);
            ammoOutUi.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !isPause)
        {
            isPause = true;
            if (isPause)
            {
                Time.timeScale = 0.025f;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            inventoryUI.SetActive(true);
        }

        else if (Input.GetKeyUp(KeyCode.Tab) && isPause)
        {
            isPause = false;
            if (!isPause)
            {
                Time.timeScale = 1f;
                inventory.IsRifleActive(selectedSlot);
            }
            inventoryUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}