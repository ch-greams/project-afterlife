using System.Collections;
using UnityEngine;


public class ItemInteractable : Interactable
{
    public Renderer itemRenderer;
    public CellInteractable attachedCell;

    private Color defaultColor;

	private readonly Color hoverColor = Color.cyan;


    private void Start()
    {
        this.defaultColor = this.itemRenderer.material.color;
    }

    protected override IEnumerator OnHoverStart()
    {
        yield return null;
        this.itemRenderer.material.color = this.hoverColor;
    }

    protected override IEnumerator OnHoverEnd()
    {
        yield return new WaitForSeconds(0.1F);
        this.itemRenderer.material.color = this.defaultColor;
    }
}
