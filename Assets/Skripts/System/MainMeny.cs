using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMeny : MonoBehaviour
{
    public static MainMeny inctance;
    public bool peremenay;
    public GameObject countiniun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        inctance = this;

        string path = Application.persistentDataPath + "/Player.save";

        Debug.Log(Application.persistentDataPath);

        if (File.Exists(path))
        {
            countiniun.SetActive(true);
        }
        else
        {
            countiniun.SetActive(false);
        }
    }
    public void GameStart()
    {
        peremenay = false;
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    public void GameContinue()
    {
        peremenay = true;
        SceneManager.LoadScene(1);
    }

    public void GameQuit()
    {
        Application.Quit();
    }    

}
