using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ContainerInteractable : Interactable
{
    public ContainerType type;
    public Renderer itemRenderer;
    public List<TileInteractable> attachedTiles = new List<TileInteractable>();
    public PlayerController playerControl;

    public SceneStateController sceneState;

    public Color defaultColor;
	public Color hoverColor = Color.cyan;

    private readonly int ATTEMPT_TAKE_HASH = Animator.StringToHash("AttemptTake");
    private readonly int MED_TAKE_HASH = Animator.StringToHash("MedTake");
    private readonly WaitForSeconds CONTAINER_OPEN_TIMEOUT = new WaitForSeconds(1.5F);


    protected override IEnumerator OnLeftClick()
    {
        switch (this.GetReaction())
        {
            case ContainerReaction.GO_TO_CONTAINER:
                yield return GoToClosestAttachedTile();
                break;
            case ContainerReaction.TRY_OPEN_CONTAINER:
                yield return GoToClosestAttachedTile();
                TryOpenContainer();
                break;
            case ContainerReaction.OPEN_CONTAINER:
                yield return GoToClosestAttachedTile();
                yield return OpenContainer();
                break;
            default:
                Debug.Log("Unexpected ContainerReaction");
                break;
        }
    }

    private ContainerReaction GetReaction()
    {
        switch (this.type)
        {
            case ContainerType.AptN1_Bedroom_Table:
                if (this.sceneState.globalState.containers[this.type].Any()) {
                    return ContainerReaction.OPEN_CONTAINER;
                }
                else {
                    Debug.Log("AptN1_Bedroom_Table is empty");
                    return ContainerReaction.TRY_OPEN_CONTAINER;
                }
            case ContainerType.AptN1_Bedroom_Bed:
            default:
                Debug.Log("Unexpected ContainerType");
                return ContainerReaction.GO_TO_CONTAINER;
        }
    }

    private IEnumerator GoToClosestAttachedTile()
    {
        if (this.attachedTiles.Any())
        {
            this.attachedTiles.Sort((ti1, ti2) =>
            {
                double est1 = ti1.tile.point.EstimateTo(this.playerControl.currentTile.tile.point);
                double est2 = ti2.tile.point.EstimateTo(this.playerControl.currentTile.tile.point);
                return est1.CompareTo(est2);
            });
            yield return base.StartCoroutine(this.attachedTiles[0].MoveToThisTile());
        }
    }

    private void TryOpenContainer()
    {
        this.playerControl.Interact(ATTEMPT_TAKE_HASH, this.transform.position);
    }

    private IEnumerator OpenContainer()
    {
        this.playerControl.Interact(MED_TAKE_HASH, this.transform.position);
        yield return CONTAINER_OPEN_TIMEOUT;

        this.AddItemsToInventory();
    }

    // TODO: Rework this
    private void AddItemsToInventory()
    {
        GlobalStateController globalState = this.sceneState.globalState;

        globalState.playerInventory.AddRange(globalState.containers[this.type]);

        // TODO: Create serializable InventorySlot class to keep invormation about slot state
        globalState.playerInventorySlots[0].gameObject.SetActive(true);
        globalState.playerInventorySlots[0].sprite = globalState.containers[this.type][0].icon;

        globalState.containers[this.type].ForEach(item => Debug.Log("Added to inventory: " + item.label));
        globalState.containers[this.type] = new List<Item>();
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


public enum ContainerType
{
    AptN1_Bedroom_Table,
    AptN1_Bedroom_Bed,
}

public enum ContainerReaction
{
    GO_TO_CONTAINER,
    TRY_OPEN_CONTAINER,
    OPEN_CONTAINER,
}
