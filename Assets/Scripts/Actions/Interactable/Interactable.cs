using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Interactable : SerializedMonoBehaviour
{
    public bool showGizmo = true;

    public InteractableData data;

    [FoldoutGroup("Interactable Config", expanded: false)]
    public SceneController sceneCtrl;
    
    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> initializeActions = new List<InteractableAction>();

    [FoldoutGroup("Interactable Config", expanded: false)]
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
}
