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


    public Player() { }
}
