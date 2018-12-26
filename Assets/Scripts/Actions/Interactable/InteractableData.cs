using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class InteractableData
{
    [FoldoutGroup("Interactable Data")]
    public bool isInteractableActive;
    [FoldoutGroup("Interactable Data")]
    public GameObject interactableObject;

    [FoldoutGroup("Interactable Data"), HideInInspector]
    public Renderer renderer;
    [FoldoutGroup("Interactable Data"), HideInInspector]
    public Animator animator;
    [FoldoutGroup("Interactable Data"), HideInInspector]
    public Color defaultColor;

    [FoldoutGroup("Interactable Data")]
    public List<Point> reachablePoints = new List<Point>();


    [FoldoutGroup("Interactable Data"), Button(ButtonSizes.Medium)]
    public void ToggleInteractable()
    {
        this.ToggleInteractable(!this.isInteractableActive);
    }

    public void ToggleInteractable(bool enable)
    {
        this.isInteractableActive = enable;

        if (this.interactableObject)
        {
            this.interactableObject.SetActive(this.isInteractableActive);
        }
    }

}
