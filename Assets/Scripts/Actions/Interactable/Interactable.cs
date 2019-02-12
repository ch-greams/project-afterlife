using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Interactable : SerializedMonoBehaviour
{
    public bool showGizmo = true;

    public InteractableData data;

    [FoldoutGroup("Interactable Config", expanded: false, order: 1)]
    public SceneController sceneCtrl;
    
    [FoldoutGroup("Interactable Config", expanded: false, order: 1)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> initializeActions = new List<InteractableAction>();

    [FoldoutGroup("Interactable Config", expanded: false, order: 1)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> clickActions = new List<InteractableAction>();


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
            this,
            this.sceneCtrl.sceneState.isDungeonScene
        );
    }

    private void OnTriggerExit(Collider collider)
    {
        this.sceneCtrl.globalCtrl.playerActionManager.TrySelectInteractable(
            null,
            this.sceneCtrl.sceneState.isDungeonScene
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

    [BoxGroup("Interactable Controls", order: 0), ShowInInspector]
    private bool enableInteractions = false;
    [BoxGroup("Interactable Controls", order: 0), ShowInInspector]
    private bool showInteractableObject = false;

    [BoxGroup("Interactable Controls", order: 0), Button(ButtonSizes.Medium)]
    private void ToggleInteractable()
    {
        this.ToggleInteractable(this.enableInteractions, this.showInteractableObject);
    }

    public void ToggleInteractable(bool enable, bool show)
    {
        this.data.isInteractableActive = enable;
        
        if (this.data.hasCollider)
        {
            this.GetComponent<Collider>().enabled = enable;
        } 

        if (this.data.interactableObject)
        {
            this.data.interactableObject.SetActive(show);
        }
    }
}
