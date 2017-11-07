using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 0.5f;
    public string startingSceneName = "ApartmentN1_Bedroom";

    private const float ALPHA_TRANSPARENT = 0F;
    private const float ALPHA_OPAQUE = 1F;


    private IEnumerator Start()
    {
        this.faderCanvasGroup.alpha = ALPHA_OPAQUE;
        yield return base.StartCoroutine(this.LoadSceneAndSetActive(this.startingSceneName));
        yield return base.StartCoroutine(this.Fade(ALPHA_TRANSPARENT));
    }

    public IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        Debug.Log("step1");
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("step2");
        yield return StartCoroutine(this.LoadSceneAndSetActive(sceneName));
        Debug.Log("step3");
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
    }

    private IEnumerator Fade(float finalAlpha)
    {
        this.faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(this.faderCanvasGroup.alpha - finalAlpha) / this.fadeDuration;

        while (!Mathf.Approximately(this.faderCanvasGroup.alpha, finalAlpha))
        {
            this.faderCanvasGroup.alpha = Mathf.MoveTowards(this.faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        this.faderCanvasGroup.blocksRaycasts = false;
    }
}
