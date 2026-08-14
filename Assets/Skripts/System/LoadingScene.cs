using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public float progressSmoothSpeed = 0.35f;
    public Slider progresBar;
    public GameObject loadingUi;
    public TMP_Text loadingText;

    IEnumerator Start()
    {
        progresBar.minValue = 0f;
        progresBar.maxValue = 1f;
        progresBar.value = 0f;

        UpdateLoadingUI(0f);
        //DontDestroyOnLoad(gameObject);  
        //if (loadingUi.transform.root.gameObject != gameObject)
        //{
        //    DontDestroyOnLoad(loadingUi.transform.root.gameObject);
        //}
        //Canvas loadingCanvas = loadingUi.GetComponent<Canvas>();
        //if (loadingCanvas != null)
        //{
        //    loadingCanvas.overrideSorting = true;
        //    loadingCanvas.sortingOrder = short.MaxValue;
        //}

        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        op.allowSceneActivation = false;
        float displayedProgress = 0f;

        while (op.progress < 0.9f || displayedProgress < 1f)
        {
            float sceneProgress = Mathf.Clamp01(op.progress / 0.9f);

            float targetProgress = op.progress >= 0.9f ? 1f : sceneProgress;
            float safeDeltaTime = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, progressSmoothSpeed * safeDeltaTime);

            UpdateLoadingUI(displayedProgress);
            yield return null;
        }
        UpdateLoadingUI(1f);
        yield return null;
        op.allowSceneActivation = true;
    }

    private void UpdateLoadingUI(float progress)
    {
        progresBar.value = progress;
        int percent = Mathf.RoundToInt(progress * 100);
        loadingText.text = $"Loading... {percent}%";
    }
}
