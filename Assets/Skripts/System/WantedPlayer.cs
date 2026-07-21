using System.Collections.Generic;
using UnityEngine;

public class WantedPlayer : MonoBehaviour
{

    public List<bool> wantedLevel = new List<bool> {false, false, false, false, false};
    public List<GameObject> starsLevel = new List<GameObject> { };
    public Player player;

    public bool firstDamage = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.kills >= 1 || firstDamage)
        {
            wantedLevel[0] = true;
            starsLevel[0].SetActive(true);
        }

        if (player.kills >= 5)
        {
            wantedLevel[1] = true;
            starsLevel[1].SetActive(true);
        }

        if (player.kills >= 10)
        {
            wantedLevel[2] = true;
            starsLevel[2].SetActive(true);
        }

        if (player.kills >= 17)
        {
            wantedLevel[3] = true;
            starsLevel[3].SetActive(true);
        }

        if (player.kills >= 30)
        {
            wantedLevel[4] = true;
            starsLevel[4].SetActive(true);
        }

    }
}
