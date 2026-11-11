using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int kills = 0;
    public float health = 100;
    public float maxHealth = 100;
    public float armor = 100;

    public int currentMoney = 0;

    [SerializeField] bool isISothing = false;
    [SerializeField] bool isVoidStarted = false;
    public Inventory inventory;
    public Misions misions;

    [SerializeField] Vechicle car;

    private int timer = 0;

    [SerializeField] GameObject boss;

    [SerializeField] TMP_Text moneyText;

    private int lastMoney = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        car = FindFirstObjectByType<Vechicle>();
        if (MainMeny.peremenay)
        {
            LoadPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isISothing && isVoidStarted)
        {
            isVoidStarted = true;
            EvryTimeHeted();
        }

        if (currentMoney != lastMoney)
        {
            lastMoney = currentMoney;
            moneyText.text = "Money: " + currentMoney + "$";
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        currentMoney = data.playerMoney;

        misions.cuurentMission = data.mission;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;

        kills = data.kills;

        health = data.currentHelth;

        for (int i = 0; i < inventory.isWeaponPicked.Length; i++)
        {
            inventory.isWeaponPicked[i] = data.isWeaponPicked[i];
        }

        car.gameObject.SetActive(true);

        if (misions.cuurentMission == 4)
        {
            boss.SetActive(true);
        }
    }

    public void PlayerGetDamage(float takeDamage)
    {
        if (armor <= 0)
        {
            if (armor <= 0)
            {
                health -= takeDamage;
                isISothing = true;
            }
        }

        else if (armor >= 1)
        {
            armor -= takeDamage;
            health -= takeDamage % 4;
            isISothing = true;
        }

        //playerBar.SetArmor(armor);
        //playerBar.SetHealth(health);

        if (health <= 0f)
        {
            Death();
        }

    }

    void Death()
    {
        print("Òû ëîõ");
        Object.Destroy(gameObject, 1f);
    }

    void EvryTimeHeted()
    {
        timer++;

        if (timer >= 5)
        {
            timer = 0;
            isVoidStarted = false;
            return;
        }

        PlayerGetDamage(1f);
        Invoke("EvryTimeHeted", 1f);
    }

}