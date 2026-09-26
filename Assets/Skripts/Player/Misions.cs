using System.Linq;
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
        currentMission.text = "Curent mision: " + (cuurentMission + 1);
        currentMissionText.text = misionsText[cuurentMission];
    }

    public void NextMision()
    {
        cuurentMission++;

        if (cuurentMission < misionsText.Length)
        {
            for (int i = 0; i < misionsText.Length; i++)
            {
                if (cuurentMission == i)
                {
                    print(misionsText[i]);
                }
            }

            currentMission.text = "Curent mision: " + (cuurentMission + 1);
            currentMissionText.text = misionsText[cuurentMission];
        }
    }
}
