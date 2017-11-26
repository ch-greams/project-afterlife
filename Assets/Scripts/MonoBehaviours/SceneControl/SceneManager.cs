using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneManager
{
    private static GlobalController globalCtrl;
    private static bool isFading;

    private const float ALPHA_TRANSPARENT = 0F;
    private const float ALPHA_OPAQUE = 1F;


    public static IEnumerator Init(GlobalController globalCtrl, CanvasGroup faderCanvasGroup, string sceneName)
    {
        SceneManager.globalCtrl = globalCtrl;
        faderCanvasGroup.alpha = ALPHA_OPAQUE;

        yield return SceneManager.FadeAndSwitchScenes(sceneName, false);
    }

    public static void FadeAndLoadScene(string sceneName)
    {
        if (!SceneManager.isFading)
        {
            SceneManager.globalCtrl.StartCoroutine(SceneManager.FadeAndSwitchScenes(sceneName, true));
        }
    }

    private static IEnumerator FadeAndSwitchScenes(string sceneName, bool unloadCurrent)
    {
        if (unloadCurrent)
        {
            yield return SceneManager.globalCtrl.StartCoroutine(SceneManager.Fade(ALPHA_OPAQUE));
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        yield return SceneManager.globalCtrl.StartCoroutine(SceneManager.LoadSceneAndSetActive(sceneName));
        yield return SceneManager.globalCtrl.StartCoroutine(SceneManager.Fade(ALPHA_TRANSPARENT));
    }

    private static IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(UnityEngine.SceneManagement.SceneManager.sceneCount - 1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(loadedScene);
    }

    private static IEnumerator Fade(float finalAlpha)
    {
        CanvasGroup faderCanvasGroup = SceneManager.globalCtrl.faderCanvasGroup;

        SceneManager.isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / SceneManager.globalCtrl.fadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        SceneManager.isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }
}
