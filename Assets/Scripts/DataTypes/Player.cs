using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Player
{
    public float animationSpeed = 4.0F;
    public bool isMoving = false;
    public bool isTargetUpdating = false;
    public Transform playerTransform;
    public Transform characterTransform;
    public GameObject flashlightRay;
    public GameObject tileSelector;
    public Animator characterAnimator;
    public Tile tile;
    public float visibleRange = 2.5F;
    public float flashlightRange = 5.0F;

    // TODO: Clean up this
    public Dictionary<Vector3, Tile> activeTiles = new Dictionary<Vector3, Tile>();

    private GlobalController globalCtrl;
    private int speedParamHash;


    public Player() { }

    public void InitPlayer(GlobalController globalCtrl, Tile tile, float visibleRange)
    {
        this.tile = tile;
        this.playerTransform.position = tile.obj.transform.position;
        this.visibleRange = visibleRange;

        this.globalCtrl = globalCtrl;
        this.speedParamHash = Animator.StringToHash("Speed");
    }

    /// <summary>
    /// Move player to selected tile and update visible tiles
    /// </summary>
    /// <param name="tile">Tile to move player to</param>
    /// <returns>IEnumerator for Coroutine</returns>
    public IEnumerator MoveToTile(Tile tile)
    {
        HashSet<Point> _playerPoints = this.GetVisiblePoints();

        Path<Tile> path = tile.FindPathFrom(this.tile, (t) => (!t.isBlocked && t.isVisible && t.isActive));
        yield return this.MoveOnPath(path.Reverse());

        this.HighlightVisible(false, _playerPoints);
    }

    /// <summary>
    /// Move player on provided path of tiles
    /// </summary>
    /// <param name="path">Path for player to walk on</param>
    /// <returns>IEnumerator for Coroutine</returns>
    private IEnumerator MoveOnPath(IEnumerable<Tile> path)
    {
        foreach (Tile tile in path)
        {
            this.characterAnimator.SetFloat(this.speedParamHash, this.animationSpeed * 1F);

            float startTime = Time.time;
            Vector3 startPosition = this.playerTransform.position;
            Vector3 endPosition = new Vector3(tile.obj.transform.position.x, 0, tile.obj.transform.position.z);
            float journeyLength = Vector3.Distance(startPosition, endPosition);

            this.characterTransform.LookAt(endPosition);

            while (endPosition != this.playerTransform.position)
            {
                float distCovered = (Time.time - startTime) * this.animationSpeed;
                float fracJourney = distCovered / journeyLength;
                this.playerTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

                yield return null;
            }

            this.globalCtrl.UpdatePlayerPosition(tile.point);

            this.tile.isBlockedByPlayer = false;
            this.tile = tile;
            this.tile.isBlockedByPlayer = true;

            this.characterAnimator.SetFloat(this.speedParamHash, 0F);
        }
    }


    private HashSet<Point> GetVisiblePoints()
    {
        return this.tile.GetTiles(this.visibleRange, (t) => (true));
    }

    // TODO: Add limit for max visibleRange
    public void ChangeVisibleRange(float visibleRange)
    {
        if (visibleRange >= 1.5F)
        {
            this.HighlightVisible(false);
            this.visibleRange = visibleRange;
            this.HighlightVisible(true);

            this.globalCtrl.UpdatePlayerVisibility(this.visibleRange);
        }
        else
        {
            Debug.Log("YOU DIED");
        }
    }

    public void HighlightVisible(bool enable, HashSet<Point> _playerPoints = null)
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;

        HashSet<Point> playerPoints = (
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

    public void HighlightActive(bool enable, bool useFullRange, HashSet<Point> _playerPoints = null)
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;
        float range = useFullRange ? this.visibleRange : 1.5F;

        HashSet<Point> playerPoints = (
            _playerPoints == null
                ? this.tile.GetTiles(range, (t) => (true))
                : _playerPoints
        );
        List<Tile> playerTiles = sceneCtrl.tiles.FindAll((t) => playerPoints.Contains(t.point));

        this.activeTiles = playerTiles.ToDictionary(tile => tile.obj.transform.position);

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

    /// <summary>
    /// Calculates list of tiles affected by flashlightRay and kills enemies that exists there
    /// </summary>
    /// <param name="selectedPoint">Direction in which flashlightRay will be cast</param>
    public void UseFlashlight(Point selectedPoint)
    {
        Path<Tile> rayPath = this.tile.GetTilesByDirection(selectedPoint, this.flashlightRange);
        foreach (Tile tile in rayPath)
        {
            this.globalCtrl.enemyManager.TryDestroyEnemyOnPoint(tile.point, true);
        }
    }

    public void TryCollectItem()
    {
        this.globalCtrl.collectableManager.TryCollectItem(this.tile.point);
    }
}
