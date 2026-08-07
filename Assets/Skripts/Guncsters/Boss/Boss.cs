using UnityEngine;

public class Boss : MonoBehaviour
{

    [SerializeField] Animator anim;

    public float bossHealth;
    public float bossArmor;

    public bool isVoidStarted = false;

    public Misions misionsEmpy;
    public Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BossGetDamage(float takeDamage)
    {
        if (bossHealth > 0)
        {
            anim.SetBool("Shooting", true);
        }

        if (!isVoidStarted)
        {
            isVoidStarted = true;
            EvryTimeHeted();
        }

        if (bossArmor <= 0)
        {
            if (bossArmor <= 0)
                bossHealth -= takeDamage;
        }

        else if (bossArmor >= 1)
        {
            bossArmor -= takeDamage;
            bossHealth -= takeDamage % 4;
        }

        if (bossHealth <= 0f)
        {
            Death();
        }
    }

    void Death()
    {
        anim.SetBool("Died", true);
        anim.SetBool("Shooting", false);
        misionsEmpy.NextMision();
        player.currentMoney += 120000;

        Destroy(gameObject, 7f);
    }

    void EvryTimeHeted()
    {
        BossGetDamage(1f);
        if (bossHealth > 0)
        {
            Invoke("EvryTimeHeted", 1f);
        }
    }
}
