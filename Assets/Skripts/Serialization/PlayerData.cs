using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class PlayerData
{

    public int playerMoney;
    public float[] position;
    public bool[] isWeaponPicked;
    public int mission;
    public int kills;
    public float currentHelth;

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

        isWeaponPicked = new bool[player.inventory.isWeaponPicked.Length];
        Debug.Log(isWeaponPicked.Length);
        for (int i = 0; i < isWeaponPicked.Length; i++)
        {
            isWeaponPicked[i] = player.inventory.isWeaponPicked[i];
        }

        mission = player.misions.cuurentMission;

        kills = player.kills;
        currentHelth = player.health;
    }
}
