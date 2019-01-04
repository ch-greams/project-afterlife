﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class InteractableData
{
    [BoxGroup("Interactable Data")]
    public bool isInteractableActive;
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

    [BoxGroup("Interactable Data"), HideIf("hasCollider")]
    public List<Point> reachablePoints = new List<Point>();

}
