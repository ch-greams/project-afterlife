using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float flashlightRange = 5.0F;

    private GlobalController globalCtrl;
    private int speedParamHash;


    public Player() { }

    public void InitPlayer(GlobalController globalCtrl, Tile tile, float currentVisibility)
    {
        this.tile = tile;
        this.playerTransform.position = tile.obj.transform.position;
        this.visibleRange = currentVisibility;

        this.globalCtrl = globalCtrl;
        this.speedParamHash = Animator.StringToHash("Speed");
    }


    public IEnumerator MoveToTile(Tile tile)
    {
        List<Point> _playerPoints = this.GetVisiblePoints();

        Path<Tile> path = tile.FindPathFrom(this.tile, (t) => (!t.isBlocked && t.isVisible && t.isActive));
        yield return this.MoveOnPath(path.Reverse());

        this.HighlightVisible(false, _playerPoints);
    }

    private IEnumerator MoveOnPath(IEnumerable<Tile> path)
    {
        foreach (Tile tile in path)
        {
            this.characterAnimator.SetFloat(this.speedParamHash, this.speed * 1F);

            float startTime = Time.time;
            Vector3 startPosition = this.playerTransform.position;
            Vector3 endPosition = new Vector3(tile.obj.transform.position.x, 0, tile.obj.transform.position.z);
            float journeyLength = Vector3.Distance(startPosition, endPosition);

            this.characterTransform.LookAt(endPosition);

            while (endPosition != this.playerTransform.position)
            {
                float distCovered = (Time.time - startTime) * this.speed;
                float fracJourney = distCovered / journeyLength;
                this.playerTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

                yield return null;
            }

            this.globalCtrl.UpdatePlayerPosition(tile.point);

            // NOTE: Currently isBlocked is unnnecessary in this case
            // this.tile.isBlocked = false;
            this.tile = tile;
            // this.tile.isBlocked = true;

            this.characterAnimator.SetFloat(this.speedParamHash, 0F);
        }
    }


    private List<Point> GetVisiblePoints()
    {
        return this.tile.GetTiles(this.visibleRange, (t) => (true));
    }

    public void HighlightVisible(bool enable, List<Point> _playerPoints = null)
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;

        List<Point> playerPoints = (
            _playerPoints == null
                ? this.tile.GetTiles(this.visibleRange, (t) => (true))
                : _playerPoints
        );
        List<Tile> playerTiles = sceneCtrl.tiles.FindAll((t) => playerPoints.Contains(t.point));

        foreach (Tile tile in playerTiles)
        {
            if (enable)
            {
                tile.RefreshTileState(enable, tile.isBlocked);
            }
            else
            {
                tile.RefreshTileState(enable, tile.isBlocked, enable, false);
            }
        }

        foreach (string lightSourceId in sceneCtrl.highlightedTiles.Keys)
        {
            sceneCtrl.UpdateHighlightedTiles(lightSourceId);
        }
    }

    public void HighlightActive(bool enable, bool useFullRange, List<Point> _playerPoints = null)
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;
        float range = useFullRange ? this.visibleRange : 1.5F;

        List<Point> playerPoints = (
            _playerPoints == null
                ? this.tile.GetTiles(range, (t) => (true))
                : _playerPoints
        );
        List<Tile> playerTiles = sceneCtrl.tiles.FindAll((t) => playerPoints.Contains(t.point));

        foreach (Tile tile in playerTiles)
        {
            bool isSelected = enable ? tile.isSelected : enable;
            tile.RefreshTileState(tile.isVisible, tile.isBlocked, enable, isSelected);
        }

        foreach (string lightSourceId in sceneCtrl.highlightedTiles.Keys)
        {
            sceneCtrl.UpdateHighlightedTiles(lightSourceId);
        }
    }
}
