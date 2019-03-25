using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class InteractableData
{
    [BoxGroup("Interactable Data")]
    public string actionLabel;

    [BoxGroup("Interactable Data"), ReadOnly]
    public bool isInteractableActive;
    [BoxGroup("Interactable Data")]
    public bool isAutoTriggerInteractable;
    [BoxGroup("Interactable Data")]
    public GameObject interactableObject;

    // TODO: Remove later if necessary
    [BoxGroup("Interactable Data"), HideInInspector]
    public Animator animator;
    // TODO: Remove later if necessary
    [BoxGroup("Interactable Data"), HideInInspector]
    public Color defaultColor;

    [BoxGroup("Interactable Data")]
    public bool hasCollider = false;

    [BoxGroup("Interactable Data"), HideIf("hasCollider"), ListDrawerSettings(IsReadOnly = true), ReadOnly]
    public List<Point> reachablePoints = new List<Point>();
}

[Serializable, InlineProperty(LabelWidth = 80)]
public class InteractableState
{
    [HorizontalGroup]
    public bool enabled;
    [HorizontalGroup]
    public bool visible;


    public InteractableState(bool enabled, bool visible)
    {
        this.enabled = enabled;
        this.visible = visible;
    }


    public void Update(bool enabled, bool visible)
    {
        this.enabled = enabled;
        this.visible = visible;
    }
}
