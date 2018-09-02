using UnityEngine;


public class Player
{
    public float speed = 10.0F;
    public bool isMoving = false;
    public bool isTargetUpdating = false;
    public Transform playerTransform;
    public Transform characterTransform;
    public GameObject flashlightRay;
    public Animator characterAnimator;
    public Tile tile;
    public float visibleRange = 2.5F;


    public Player() { }

    public void UpdatePlayer(Tile tile, float currentVisibility)
    {
        this.tile = tile;
        this.playerTransform.position = tile.obj.transform.position;
        this.visibleRange = currentVisibility;
    }
}
