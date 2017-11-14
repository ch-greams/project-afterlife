using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 0.5f;
    public SceneType startingScene = SceneType.AptN1_Bedroom;

    private bool isFading;

    private const float ALPHA_TRANSPARENT = 0F;
    private const float ALPHA_OPAQUE = 1F;


    private IEnumerator Start()
    {
        this.faderCanvasGroup.alpha = ALPHA_OPAQUE;
        yield return base.StartCoroutine(this.LoadSceneAndSetActive(this.startingScene.ToString()));
        yield return base.StartCoroutine(this.Fade(ALPHA_TRANSPARENT));
    }

    public void FadeAndLoadScene(string sceneName)
    {
        if (!this.isFading)
        {
            base.StartCoroutine(this.FadeAndSwitchScenes(sceneName));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        yield return base.StartCoroutine(Fade(1f));
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return base.StartCoroutine(this.LoadSceneAndSetActive(sceneName));
        yield return base.StartCoroutine(Fade(0f));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
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
