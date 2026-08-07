using TMPro;
using UnityEngine;

public class Misions : MonoBehaviour
{

    public int cuurentMission = 0;
    public string[] misionsText;
    public TMP_Text currentMission;
    public TMP_Text currentMissionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMission.text = "Curent mision: " + cuurentMission;
        currentMissionText.text = misionsText[cuurentMission];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextMision()
    {
        cuurentMission++;
        for (int i = 0; i < misionsText.Length; i++)
        {
            if (cuurentMission == i)
            {
                print(misionsText[i]);
            }
        }
        currentMission.text = "Curent mision: " + cuurentMission;
        currentMissionText.text = misionsText[cuurentMission];
    }
}
