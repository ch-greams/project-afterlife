using System.Linq;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class GlobalController : SerializedMonoBehaviour
{

    [BoxGroup("Configuration")]
    public float fadeDuration = 0.5f;
    [BoxGroup("Configuration")]
    public CanvasGroup faderCanvasGroup;


    [InlineEditor(Expanded = true)]
    public GlobalState globalState;


    [FoldoutGroup("Interface Management")]
    public Inventory inventory;

    [FoldoutGroup("Interface Management")]
    public DialogueManager dialogueManager;

    [FoldoutGroup("Interface Management")]
    public SaveManager saveManager;

    [FoldoutGroup("Interface Management")]
    public ObjectiveManager objectiveManager;

    [FoldoutGroup("Interface Management")]
    public PlayerActionManager playerActionManager;

    [FoldoutGroup("State Management")]
    public EnemyManager enemyManager;

    [FoldoutGroup("State Management")]
    public CollectableManager collectableManager;

    [FoldoutGroup("State Management")]
    public EventTriggerManager eventTriggerManager;


    // TODO: Update this shit
    public SceneController sceneCtrl;
    public bool directionSwitch = false;
    public bool directionVerticalSignSwitch = false;
    public bool directionHorizontalSignSwitch = false;


    private void Awake()
    {
        this.globalState = CreatePlayModeInstance(this.globalState);
        this.globalState.objectives = this.globalState.objectives
            .ToDictionary(kvp => kvp.Key, kvp => GlobalController.CreatePlayModeInstance(kvp.Value));
        this.globalState.sceneStates = this.globalState.sceneStates
            .ToDictionary(kvp => kvp.Key, kvp => GlobalController.CreatePlayModeInstance(kvp.Value));

        // Interface Management
        this.inventory.LoadFromState(this);
        this.dialogueManager.Init();
        this.saveManager.Init(this);
        this.objectiveManager.Init(this);
        this.playerActionManager.Init(this);
        // State Management
        this.enemyManager.Init(this);
        this.collectableManager.Init(this);
        this.eventTriggerManager.Init(this);
    }

    private IEnumerator Start()
    {
        yield return SceneManager.Init(this, this.faderCanvasGroup, this.globalState.currentScene.ToString());
    }

    private void Update()
    {
        if (!SceneManager.sceneLoadingInProgress)
        {
            StartCoroutine(this.playerActionManager.InputListener());
        }
    }

    // TODO: Move it somewhere more appropriate
    public int turnCount = 0;

    public void NextTurn()
    {
        this.turnCount++;
        base.StartCoroutine(this.NextTurnActions());
    }

    private IEnumerator NextTurnActions()
    {
        List<IWithEndOfTurnAction> managersWithNextTurnAction = new List<IWithEndOfTurnAction>()
        {
            this.playerActionManager,
            this.collectableManager,
            this.enemyManager
        };

        foreach (IWithEndOfTurnAction manager in managersWithNextTurnAction)
        {
            yield return manager.OnTurnChange();
        }

        foreach (EndOfTurnAction endOfTurnAction in this.eventTriggerManager.endOfTurnActions)
        {
            if (endOfTurnAction.IsValid()) {
                yield return endOfTurnAction.React();
            }
        }
    }

    public void LoadFromState()
    {
        this.objectiveManager.Init(this);
        SceneManager.FadeAndLoadScene(this.globalState.currentScene.ToString());
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

    public void UpdatePlayerVisibility(float currentVisibility)
    {
        this.globalState.currentVisibility = currentVisibility;
    }


    public static T CreatePlayModeInstance<T>(T assetState) where T : ScriptableObject
    {
        T instance = ScriptableObject.Instantiate<T>(assetState);
        instance.name = string.Format("[PLAY_MODE] {0}", assetState.name);
        return instance;
    }
}
