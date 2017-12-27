using System.Linq;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;


public class GlobalController : SerializedMonoBehaviour
{
    [BoxGroup("Configuration")]
    public SceneType startingScene = SceneType.AptN1_Bedroom;

    [BoxGroup("Configuration")]
    public float fadeDuration = 0.5f;


    [InlineEditor(Expanded = true)]
    public GlobalState globalState;


    public Inventory inventory;

    public DialogManager dialogManager;

    public ObjectiveManager objectiveManager;

    [BoxGroup("User Interface")]
    public CanvasGroup faderCanvasGroup;


    private void Awake()
    {
        this.globalState = CreatePlayModeInstance(this.globalState);
        this.globalState.objectives = this.globalState.objectives
            .ToDictionary(kvp => kvp.Key, kvp => GlobalController.CreatePlayModeInstance(kvp.Value));

        this.inventory.LoadFromState(this);
        this.dialogManager.Init();
        this.objectiveManager.Init(this.globalState);
    }

    private IEnumerator Start()
    {
        yield return SceneManager.Init(this, this.faderCanvasGroup, this.startingScene.ToString());
    }

    public void UpdatePlayerPosition(SceneType scene, Point position)
    {
        this.globalState.currentPosition = position;
        this.globalState.currentScene = scene;
    }

    public void UpdatePlayerPosition(Point position)
    {
        this.globalState.currentPosition = position;
    }

    public static T CreatePlayModeInstance<T>(T assetState) where T : ScriptableObject
    {
        T instance = ScriptableObject.Instantiate<T>(assetState);
        instance.name = string.Format("[PLAY_MODE] {0}", assetState.name);
        return instance;
    }
}
