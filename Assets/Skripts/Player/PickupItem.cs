using UnityEngine;

public class PickupItem : MonoBehaviour
{

    [Header("Item Info")]
    public int itemPrice;
    public int itemRadius;
    public bool handGun;

    public Rifle rifle;
    public HandGunRifle handGunRifle;

    [Header("Player Info")]
    public Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= itemRadius)
        {
            if (Input.GetKeyDown(KeyCode.E))
            { 
                if (player.currentMoney >= itemPrice)
                {
                    player.currentMoney -= itemPrice;
                    if (handGun == false)
                    {
                        rifle.isItemInInfertory = true;
                    }
                    else
                    {
                        handGunRifle.isItemInInfertoryHandGunEdition = true;
                    }
                    gameObject.SetActive(false);
                }
                else
                {
                    print("’ј’ј’ј’’ј’, лох иди капать шахиту за 3 рубл€ в год");
                }
            }
        }
    }
}
