﻿using System.Collections;
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
    public List<Action> leftClickActions = new List<Action>();

    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<Action> hoverStartActions = new List<Action>();

    [FoldoutGroup("Interactable Config", expanded: false)]
    [ListDrawerSettings(ListElementLabelName = "name", Expanded = false)]
    public List<Action> hoverEndActions = new List<Action>();


    private void Awake()
    {
        base.StartCoroutine(this.OnInit());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
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
        this.leftClickActions.ForEach(action => action.Init(this));
        this.hoverStartActions.ForEach(action => action.Init(this));
        this.hoverEndActions.ForEach(action => action.Init(this));

        yield return null;
    }
    private IEnumerator OnLeftClick()
    {
        Action action = this.leftClickActions.Find(a => a.IsValid());

        if (action != null)
        {
            yield return action.React();
        }
    }
    private IEnumerator OnRightClick()
    {
        yield return null;
    }
    private IEnumerator OnHoverStart()
    {
        Action action = this.hoverStartActions.Find(a => a.IsValid());

        if (action != null)
        {
            yield return action.React();
        }
    }
    private IEnumerator OnHoverEnd()
    {
        Action action = this.hoverEndActions.Find(a => a.IsValid());

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
    Color defaultColor { get; }
    List<Tile> neighbourTiles { get; }
}
