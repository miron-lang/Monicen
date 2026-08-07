using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{

    public int playerMoney;
    public float[] position;
    public bool[] isWeaponPicked;
    public int mission;
    public float kills;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerData(Player player)
    {
        playerMoney = player.currentMoney;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        isWeaponPicked = new bool[6];
        for (int i = 0; i < 6; i++)
        {
            isWeaponPicked[i] = player.inventory.isWeaponPicked[i];
        }

        mission = player.misions.cuurentMission;

            kills = player.kills;
    }
}
