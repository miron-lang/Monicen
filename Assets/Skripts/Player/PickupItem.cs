using UnityEngine;

public class PickupItem : MonoBehaviour
{

    [Header("Item Info")]
    public int itemRadius;
    private string itemTag;

    [Header("Player Info")]
    public Inventory inventory;
    public Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemTag = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= itemRadius)
        {
            if (itemTag == "HandGunPickup")
            {
                inventory.isWeaponPicked[0] = true;
            }
            else if (itemTag == "HandGunWhithAMuferPickup")
            {
                inventory.isWeaponPicked[1] = true;
            }
            else if (itemTag == "UziPickup")
            {
                inventory.isWeaponPicked[2] = true;
            }
            else if (itemTag == "ShootGunPickup")
            {
                inventory.isWeaponPicked[3] = true;
            }
            else if (itemTag == "BazukaPickup")
            {
                inventory.isWeaponPicked[4] = true;
            }
            else if (itemTag == "BayoutPickup")
            {
                inventory.isWeaponPicked[5] = true;
            }
            gameObject.SetActive(false);
        }
    }
    
}
