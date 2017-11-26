using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class GlobalController : SerializedMonoBehaviour
{
    [BoxGroup("Configuration")]
    public SceneType startingScene = SceneType.AptN1_Bedroom;

    [BoxGroup("Configuration")]
    public float fadeDuration = 0.5f;


    [InlineEditor(Expanded = true)]
    public GlobalState globalState;


    [BoxGroup("User Interface")]
    public List<Image> playerInventorySlots = new List<Image>();

    [BoxGroup("User Interface")]
    public CanvasGroup faderCanvasGroup;

    private void Awake()
    {
        this.globalState = SerializedScriptableObject.Instantiate(this.globalState);
    }

    private IEnumerator Start()
    {
        yield return SceneManager.Init(this, this.faderCanvasGroup, this.startingScene.ToString());
    }
}

[Flags]
public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
}
