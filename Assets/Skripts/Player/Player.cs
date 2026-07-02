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

    public PlayerBar playerBar;

    void Start()
    {
        // Перенесли сюда из Awake, чтобы PlayerBar успел проснуться
        if (playerBar != null)
        {
            playerBar.SetArmor(armor);
            playerBar.SetHealth(health);
        }
    }

    void Update()
    {
        if (isISothing && isVoidStarted)
        {
            isVoidStarted = true;
            EvryTimeHeted();
        }
    }

    public void PlayerGetDamage(float takeDamage)
    {
        if (armor <= 0)
        {
            health -= takeDamage;
            isISothing = true;
        }
        else if (armor >= 1)
        {
            armor -= takeDamage;
            health -= takeDamage % 4;
            isISothing = true;
        }

        if (playerBar != null)
        {
            playerBar.SetArmor(armor);
            playerBar.SetHealth(health);
        }

        if (health <= 0f)
        {
            Death();
        }
    }

    void Death()
    {
        print("Ты лох");
        Object.Destroy(gameObject, 1f);
    }

    void EvryTimeHeted()
    {
        PlayerGetDamage(1f);
        Invoke("EvryTimeHeted", 1f);
    }
}