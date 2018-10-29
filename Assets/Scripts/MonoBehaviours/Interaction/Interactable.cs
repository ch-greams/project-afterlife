using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;


public class Interactable : SerializedMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public IDataInteractable data;

    [FoldoutGroup("Interactable Config", expanded: false)]
    public SceneController sceneCtrl;
    
    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> initializeActions = new List<InteractableAction>();

    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> leftClickActions = new List<InteractableAction>();

    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> hoverStartActions = new List<InteractableAction>();

    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<InteractableAction> hoverEndActions = new List<InteractableAction>();


    private void Awake()
    {
        base.StartCoroutine(this.OnInit());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0) || Input.GetButtonDown("Button A"))
        {
            base.StartCoroutine(this.OnLeftClick());
        }
        else if (Input.GetMouseButtonUp(1))
        {
            base.StartCoroutine(this.OnRightClick());
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        base.StartCoroutine(this.OnHoverStart());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        base.StartCoroutine(this.OnHoverEnd());
    }

    private IEnumerator OnInit()
    {
        this.initializeActions.ForEach(action => action.Init(this));
        this.leftClickActions.ForEach(action => action.Init(this));
        this.hoverStartActions.ForEach(action => action.Init(this));
        this.hoverEndActions.ForEach(action => action.Init(this));

        yield return this.TriggerValidAction(this.initializeActions);
    }
    private IEnumerator OnLeftClick()
    {
        yield return this.TriggerValidAction(this.leftClickActions);
    }
    private IEnumerator OnRightClick()
    {
        yield return null;
    }
    private IEnumerator OnHoverStart()
    {
        yield return this.TriggerValidAction(this.hoverStartActions);
    }
    private IEnumerator OnHoverEnd()
    {
        yield return this.TriggerValidAction(this.hoverEndActions);
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
    List<Tile> neighbourTiles { get; }
}
