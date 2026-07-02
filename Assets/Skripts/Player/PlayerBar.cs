using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerBar : MonoBehaviour
{
    public Slider helthBar;
    public Slider armorBar;

    // TMP_Text работает и с обычным TextMeshPro, и с TextMeshProUGUI на Canvas
    public TMP_Text money;

    public Player playerSkript;

    void Update()
    {
        // Ѕезопасна€ проверка: обновл€ем текст только если всЄ прив€зано в инспекторе
        if (money != null && playerSkript != null)
        {
            money.text = "$ " + playerSkript.currentMoney;
        }
    }

    public void SetHealth(float health)
    {
        if (helthBar == null) return;

        helthBar.value = health;

        // «ащита на случай, если playerSkript ещЄ не успел проинициализироватьс€
        if (playerSkript != null)
        {
            helthBar.maxValue = playerSkript.maxHealth;
        }
    }

    public void SetArmor(float armor)
    {
        if (armorBar != null)
        {
            armorBar.value = armor;
            armorBar.maxValue = 100;
        }
    }
}