using System.Collections;
using UnityEngine;

public class ProgressiveActivator : MonoBehaviour
{

    public GameObject cameraRig;
    public GameObject character;

    public static float progress = 0f;

    IEnumerator Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        int total = renderers.Length;
        int current = 0;

        foreach (var r in renderers)
            r.enabled = false;

        yield return null;

        foreach (var r in renderers)
        {
            r.enabled = true;

            current++;
            progress = (float)current / total;

            yield return null;
        }
        GameLoadState.isReady = true;
        cameraRig.SetActive(true);
        character.SetActive(true);


    }

}
