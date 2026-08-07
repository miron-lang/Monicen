using UnityEngine;

public class SaveGlow : MonoBehaviour
{
    public Misions misionsEmpty;
    public Player player;
    public GameObject car;
    public GameObject shop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (other.CompareTag("Player"))
        {
            if (misionsEmpty.cuurentMission == 0)
            {
                misionsEmpty.NextMision();
                player.currentMoney += 643;
                car.SetActive(true);
            }
            else if (misionsEmpty.cuurentMission == 2)
            {
                misionsEmpty.NextMision();
                player.currentMoney -= 125;
                shop.SetActive(true);
            }
                //Сохронений
            }
        }
}
