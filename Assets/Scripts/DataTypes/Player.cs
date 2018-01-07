using UnityEngine;


public class Player
{
    public float speed = 10.0F;
    public bool isMoving = false;
    public bool isTargetUpdating = false;
    public Transform playerTransform;
    public Transform characterTransform;
    public Animator characterAnimator;
    public Tile tile;
    public int visibleRange = 2;


    public Player() { }

    public void UpdatePlayer(Tile tile, int currentVisibility)
    {
        this.tile = tile;
        this.playerTransform.position = tile.obj.transform.position;
        this.visibleRange = currentVisibility;
    }
}
