using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMeny : MonoBehaviour
{



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    public void GameContinue()
    {
        SceneManager.LoadScene(1);
    }

    public void GameQuit()
    {
        Application.Quit();
    }    

}
