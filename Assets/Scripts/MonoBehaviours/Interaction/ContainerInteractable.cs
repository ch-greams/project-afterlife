using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ContainerInteractable : Interactable
{
    public ContainerType type;
    public Renderer itemRenderer;
    public List<Tile> attachedTiles = new List<Tile>();
    public PlayerController playerControl;

    public Container container;

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
            case ContainerReaction.TRY_OPEN_EMPTY_CONTAINER:
                Debug.Log(string.Format("{0} is empty", this.type));
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
            case ContainerType.AptN1_Bedroom_Bed:
                return this.container.IsNotEmpty()
                    ? ContainerReaction.OPEN_CONTAINER
                    : ContainerReaction.TRY_OPEN_EMPTY_CONTAINER;
            default:
                Debug.Log(string.Format("Unexpected ContainerType: {0}", this.type));
                return ContainerReaction.GO_TO_CONTAINER;
        }
    }

    private IEnumerator GoToClosestAttachedTile()
    {
        if (this.attachedTiles.Any())
        {
            this.attachedTiles.Sort((ti1, ti2) =>
            {
                double est1 = ti1.point.EstimateTo(this.playerControl.currentTile.point);
                double est2 = ti2.point.EstimateTo(this.playerControl.currentTile.point);
                return est1.CompareTo(est2);
            });

            Tile closestTile = this.attachedTiles.First();
            yield return base.StartCoroutine(closestTile.obj.GetComponent<TileInteractable>().MoveToThisTile());
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

        this.container.AddItemsToInventory();
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


public enum ContainerReaction
{
    GO_TO_CONTAINER,
    TRY_OPEN_EMPTY_CONTAINER,
    OPEN_CONTAINER,
}
