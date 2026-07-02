using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public string sceneToLoad = "DemoScene";
    public Slider progresBar;
    public GameObject loadingUi;

    IEnumerator Start()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            float sceneProgress = Mathf.Clamp01(op.progress / 0.9f);

            progresBar.value = sceneProgress * 0.5f;
            yield return null;
        }

        while (!GameLoadState.isReady)
        {
            yield return null;
        }

        Scene gameScene = SceneManager.GetSceneByName(sceneToLoad);

        SceneManager.SetActiveScene(gameScene);

        loadingUi.SetActive(false);

        SceneManager.UnloadSceneAsync("LoadScene");

    }
}
