using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;


public class GlobalController : SerializedMonoBehaviour
{

    [BoxGroup("Scene Switch Configuration")]
    public float fadeDuration = 0.5f;
    [BoxGroup("Scene Switch Configuration")]
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

    [FoldoutGroup("Interface Management"), BoxGroup("Interface Management/Player Actions")]
    public PlayerActionManager playerActionManager;

    [FoldoutGroup("Interface Management"), BoxGroup("Interface Management/Player Actions")]
    public EndOfTurnActionManager endOfTurnActionManager;

    // TODO: Do something smart about it
    [FoldoutGroup("Interface Management")]
    public StatsManager statsManager;


    [FoldoutGroup("State Management")]
    public EnemyManager enemyManager;

    [FoldoutGroup("State Management")]
    public CollectableManager collectableManager;

    // TODO: Update "Game Over" shit
    [BoxGroup("Game Over")]
    public GameObject gameOverFade;
    [BoxGroup("Game Over")]
    public bool isGameOver = false;

    // TODO: Update this shit
    public SceneController sceneCtrl;

    [BoxGroup("Player Controls")]
    public bool directionSwitch = false;
    [BoxGroup("Player Controls")]
    public bool directionVerticalSignSwitch = false;
    [BoxGroup("Player Controls")]
    public bool directionHorizontalSignSwitch = false;


    [ShowInInspector]
    public static List<string> sceneNames
    {
        get
        {
            List<string> result = new List<string>();

            for (int sceneIndex = 0; sceneIndex < UnitySceneManager.sceneCountInBuildSettings; sceneIndex++)
            {
                result.Add(
                    Path.GetFileNameWithoutExtension(
                        SceneUtility.GetScenePathByBuildIndex(sceneIndex)
                    )
                );
            }

            return result;
        }
    }


    private void Awake()
    {
        this.globalState = GlobalController.CreatePlayModeInstance(this.globalState);
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
        this.endOfTurnActionManager.Init(this);

        // State Management
        this.enemyManager.Init(this);
        this.collectableManager.Init(this);
    }

    private IEnumerator Start()
    {
        yield return SceneManager.Init(this, this.faderCanvasGroup, this.globalState.currentScene);
    }

    private void Update()
    {
        if (!SceneManager.sceneLoadingInProgress && this.sceneCtrl != null)
        {
            this.playerActionManager.InputListener(this.sceneCtrl.sceneState.isDungeonScene);
            this.dialogueManager.InputListener();

            base.StartCoroutine(this.objectiveManager.InputListener());
        }
    }

    public void SetScene(SceneController sceneCtrl)
    {
        this.sceneCtrl = sceneCtrl;

        this.playerActionManager.playerActionsGroup.SetActive(this.sceneCtrl.sceneState.isDungeonScene);
        this.playerActionManager.statsPanelGroup.SetActive(this.sceneCtrl.sceneState.isDungeonScene);
        this.playerActionManager.arePlayerControlsLocked = !this.sceneCtrl.sceneState.isDungeonScene;
    }

    public void NextTurn()
    {
        this.statsManager.IncrementTurnCount();
        base.StartCoroutine(this.endOfTurnActionManager.ReactOnValidActions());
    }

    public void LoadFromState()
    {
        this.objectiveManager.Init(this);
        SceneManager.FadeAndLoadScene(this.globalState.currentScene);
    }

    public void UpdatePlayerPosition(string scene, Point position)
    {
        this.globalState.currentScene = scene;
        this.globalState.sceneStates[scene].currentPositionPoint = position;
    }

    public void UpdatePlayerPosition(string scene, Vector3 position)
    {
        this.globalState.currentScene = scene;
        this.globalState.sceneStates[scene].currentPositionVector = position;
    }

    public void UpdatePlayerPositionPoint(Point position)
    {
        this.globalState.sceneStates[this.globalState.currentScene].currentPositionPoint = position;
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
