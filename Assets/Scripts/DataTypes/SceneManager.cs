using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;
using Sirenix.OdinInspector;


public class SceneManager : SerializedMonoBehaviour
{
    public float fadeDuration = 0.5f;
    public CanvasGroup faderCanvasGroup;

    public bool sceneLoadingInProgress { get { return this.isFading; } }

    private GlobalController globalCtrl;
    private bool isFading;


    private const float ALPHA_TRANSPARENT = 0F;
    private const float ALPHA_OPAQUE = 1F;


    public IEnumerator Init(GlobalController globalCtrl, string sceneName)
    {
        this.globalCtrl = globalCtrl;
        this.faderCanvasGroup.alpha = ALPHA_OPAQUE;

        yield return this.FadeAndSwitchScenes(sceneName, false);
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (!this.isFading)
        {
            yield return this.FadeAndSwitchScenes(sceneName, true);
        }
    }


    private IEnumerator FadeAndSwitchScenes(string sceneName, bool unloadCurrentScene)
    {
        if (unloadCurrentScene)
        {
            yield return this.globalCtrl.StartCoroutine(this.Fade(ALPHA_OPAQUE));
            yield return UnitySceneManager.UnloadSceneAsync(UnitySceneManager.GetActiveScene().buildIndex);
        }

        yield return this.globalCtrl.StartCoroutine(this.LoadSceneAndSetActive(sceneName));
        yield return this.globalCtrl.StartCoroutine(this.Fade(ALPHA_TRANSPARENT));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return UnitySceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        UnityScene loadedScene = UnitySceneManager.GetSceneAt(UnitySceneManager.sceneCount - 1);
        UnitySceneManager.SetActiveScene(loadedScene);
    }

    private IEnumerator Fade(float finalAlpha)
    {
        this.isFading = true;
        this.faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(this.faderCanvasGroup.alpha - finalAlpha) / this.fadeDuration;

        while (!Mathf.Approximately(this.faderCanvasGroup.alpha, finalAlpha))
        {
            this.faderCanvasGroup.alpha = Mathf.MoveTowards(this.faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        this.isFading = false;
        this.faderCanvasGroup.blocksRaycasts = false;
    }
}
