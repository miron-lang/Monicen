using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerBar : MonoBehaviour
{

    public Slider helthBar;
    public Slider armorBar;
    public TextMeshPro money;

    public Player playerSkript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        money.text = "$ " + playerSkript.currentMoney;
    }

    public void SetHealth(float health)
    {
        helthBar.value = health;
        helthBar.maxValue = playerSkript.maxHealth;
    }

    public void SetArmor(float armor)
    {
        armorBar.value = armor;
        armorBar.maxValue = 100;
    }

}