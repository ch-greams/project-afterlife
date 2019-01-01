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
    public EndOfTurnActionManager endOfTurnActionManager;
    public SceneController sceneCtrl;
    public bool directionSwitch = false;
    public bool directionVerticalSignSwitch = false;
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
        // Other
        this.endOfTurnActionManager.Init(this);
    }

    private IEnumerator Start()
    {
        yield return SceneManager.Init(this, this.faderCanvasGroup, this.globalState.currentScene);
    }

    private void Update()
    {
        if (!SceneManager.sceneLoadingInProgress)
        {
            this.playerActionManager.InputListener();
            this.dialogueManager.InputListener();

            base.StartCoroutine(this.objectiveManager.InputListener());
        }
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
