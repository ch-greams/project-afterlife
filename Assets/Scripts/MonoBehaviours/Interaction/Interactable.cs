using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Interactable : SerializedMonoBehaviour
{
    public IDataInteractable data;

    [FoldoutGroup("Interactable Config", expanded: false)]
    public SceneController sceneCtrl;
    
    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> initializeActions = new List<InteractableAction>();

    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> clickActions = new List<InteractableAction>();


    private void Awake()
    {
        base.StartCoroutine(this.OnInit());
    }
    public void OnClick()
    {
        base.StartCoroutine(this.TriggerValidAction(this.clickActions));
    }

    private IEnumerator OnInit()
    {
        this.initializeActions.ForEach(action => action.Init(this));
        this.clickActions.ForEach(action => action.Init(this));

        yield return this.TriggerValidAction(this.initializeActions);
    }

    private IEnumerator TriggerValidAction(List<InteractableAction> actions)
    {
        InteractableAction action = actions.Find(a => a.IsValid());

        if (action != null)
        {
            yield return action.React();
        }
    }
}

public interface IDataInteractable
{
    GameObject gameObject { get; }
    Renderer renderer { get; }
    Animator animator { get; }
    Color defaultColor { get; set; }
    List<Point> reachablePoints { get; }
}
