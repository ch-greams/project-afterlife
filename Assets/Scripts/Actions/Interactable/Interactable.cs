using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class Interactable : SerializedMonoBehaviour
{
    public bool showGizmo = true;

    [InlineProperty, HideLabel]
    public InteractableData data;


    [BoxGroup("Interactable Config"), PropertyOrder(3)]
    public SceneController sceneCtrl;
    
    [BoxGroup("Interactable Config"), PropertyOrder(4)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> initializeActions = new List<InteractableAction>();

    [BoxGroup("Interactable Config"), PropertyOrder(5)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> clickActions = new List<InteractableAction>();


    [BoxGroup("Interactable Config"), PropertyOrder(1), ShowInInspector]
    public bool IsEnabled
    {
        get { return (
            this.sceneCtrl != null && this.sceneCtrl.sceneState.interactables.ContainsKey(this.name)
                ? this.sceneCtrl.sceneState.interactables[this.name].enabled
                : false
        ); }
        set { this.ToggleInteractable(value, this.sceneCtrl.sceneState.interactables[this.name].visible); }
    }

    [BoxGroup("Interactable Config"), PropertyOrder(2), ShowInInspector]
    public bool IsVisible
    {
        get { return (
            this.sceneCtrl != null && this.sceneCtrl.sceneState.interactables.ContainsKey(this.name)
                ? this.sceneCtrl.sceneState.interactables[this.name].visible
                : false
        ); }
        set { this.ToggleInteractable(this.sceneCtrl.sceneState.interactables[this.name].enabled, value); }
    }


    private void OnDrawGizmos()
    {
        if (this.showGizmo)
        {
            Gizmos.DrawIcon(transform.position, "md-hand");
        }
    }

    private void Awake()
    {
        base.StartCoroutine(this.OnInit());
    }

    private void OnTriggerEnter(Collider collider)
    {
        this.sceneCtrl.globalCtrl.playerActionManager.TrySelectInteractable(
            interactable: this,
            isDungeonScene: this.sceneCtrl.sceneState.isDungeonScene,
            buttonLabel: this.data.actionLabel
        );
    }

    private void OnTriggerExit(Collider collider)
    {
        this.sceneCtrl.globalCtrl.playerActionManager.TrySelectInteractable(
            interactable: null,
            isDungeonScene: this.sceneCtrl.sceneState.isDungeonScene,
            buttonLabel: null
        );
    }

    public void OnClickSync()
    {
        base.StartCoroutine(this.TriggerValidActions(this.clickActions));
    }

    public IEnumerator OnClickAsync()
    {
        yield return this.TriggerValidActions(this.clickActions);
    }

    private IEnumerator OnInit()
    {
        this.initializeActions.ForEach(action => action.Init(this));
        this.clickActions.ForEach(action => action.Init(this));

        yield return this.TriggerValidActions(this.initializeActions);
    }

    private IEnumerator TriggerValidActions(List<InteractableAction> actions)
    {
        foreach (InteractableAction action in actions)
        {
            if (action.IsValid())
            {
                yield return action.React();
            }
        }
    }


    public void ToggleInteractable(bool enabled, bool visible)
    {
        this.data.isInteractableActive = enabled;
        
        if (this.data.hasCollider)
        {
            this.GetComponent<Collider>().enabled = enabled;
        } 

        if (this.data.interactableObject)
        {
            this.data.interactableObject.SetActive(visible);
        }

        if (this.sceneCtrl.sceneState.interactables.ContainsKey(this.name))
        {
            this.sceneCtrl.sceneState.interactables[this.name].Update(enabled, visible);
        }
        else
        {
            this.sceneCtrl.sceneState.interactables[this.name] = new InteractableState(enabled, visible);
        }
    }


    private bool isDungeonInteractable { get { return !this.data.hasCollider; } }

    [BoxGroup("Interactable Config"), PropertyOrder(0), Button(ButtonSizes.Medium), ShowIf("isDungeonInteractable")]
    private void UpdateReachablePoint()
    {
        ReachablePoint[] reachablePoints = this.gameObject.GetComponentsInChildren<ReachablePoint>();

        foreach (ReachablePoint reachablePoint in reachablePoints)
        {
            reachablePoint.RefreshCurrentPoint();
        }

        this.data.reachablePoints = reachablePoints.Select(rp => rp.point).ToList();
    }
}
