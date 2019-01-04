using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;


public class Player : SerializedMonoBehaviour
{
    public float animationSpeed = 4.0F;
    public bool isMoving = false;
    public bool isTargetUpdating = false;
    public Transform playerTransform;
    public Transform characterTransform;
    public Light playerSpotlight;
    public GameObject flashlightRay;
    public GameObject tileSelector;
    public Animator characterAnimator;
    public NavMeshAgent navMeshAgent;
    public Tile tile;
    public float visibleRange = 2.5F;
    public float maxVisibleRange = 4.5F;
    public float flashlightRange = 5.0F;

    // TODO: Clean up this
    public Dictionary<Vector3, Tile> activeTiles = new Dictionary<Vector3, Tile>();

    private GlobalController globalCtrl;
    private int speedParamHash;


    public Player() { }

    public void InitPlayer(GlobalController globalCtrl, Vector3 playerPosition)
    {
        this.playerTransform.position = playerPosition;

        this.globalCtrl = globalCtrl;
        this.speedParamHash = Animator.StringToHash("Speed");
    }

    public void InitPlayer(GlobalController globalCtrl, Tile tile, float visibleRange)
    {
        this.tile = tile;
        this.tile.isBlockedByPlayer = true;

        this.playerTransform.position = tile.gameObject.transform.position;
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
        Path<Tile> path = tile.FindPathFrom(this.tile, (t) => (!t.isBlocked && t.isVisible && t.isActive));

        this.UpdateHighlightOnVisible(false);

        yield return this.MoveOnPath(path.Reverse());
    }

    public void ChangeVisibleRange(float visibleRange)
    {
        if (visibleRange >= 1.5F)
        {
            this.UpdateHighlightOnVisible(false);
            this.visibleRange = (visibleRange > this.maxVisibleRange) ? this.maxVisibleRange : visibleRange;
            this.UpdateHighlightOnVisible(true);

            this.playerSpotlight.spotAngle = this.GetSpotlightAngle(
                this.visibleRange,
                this.playerSpotlight.transform.position.y
            );

            this.globalCtrl.UpdatePlayerVisibility(this.visibleRange);
        }
        else
        {
            this.globalCtrl.isGameOver = true;
        }
    }

    public void UpdateHighlightOnVisible(bool enable)
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;

        HashSet<Point> playerPoints = this.tile.GetPointsInRange(this.visibleRange, (t) => (true));
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

    public void HighlightActive(bool enable, bool useFullRange, System.Func<Tile, bool> neighbourFilter)
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;
        float range = useFullRange ? this.visibleRange : 1.5F;

        HashSet<Point> playerPoints = this.tile.GetPointsInRange(range, neighbourFilter);
        List<Tile> playerTiles = sceneCtrl.tiles.FindAll((t) => playerPoints.Contains(t.point));

        this.activeTiles = playerTiles.ToDictionary(tile => tile.gameObject.transform.position);

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
    /// Kills enemies that exists in provided tile collection
    /// </summary>
    /// <param name="selectedTiles">Tile collection to kill enemies in</param>

    public void KillEnemiesOnTiles(IEnumerable<Tile> selectedTiles)
    {
        foreach (Tile tile in selectedTiles)
        {
            this.globalCtrl.enemyManager.TryDestroyEnemyOnPoint(tile.point, true);
        }
    }

    public void TryCollectItem()
    {
        this.globalCtrl.collectableManager.TryCollectItem(this.tile.point);
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
            Vector3 endPosition = new Vector3(tile.gameObject.transform.position.x, 0, tile.gameObject.transform.position.z);
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

    private float GetSpotlightAngle(float visibleRange, float spotlightHeight)
    {
        float angleInRadians = Mathf.Atan((visibleRange + 0.5F) / spotlightHeight);

        float angleInDegrees = (180 / Mathf.PI) * angleInRadians;

        return angleInDegrees * 2;
    }

    private void MovePlayer(string horizontalButton, string verticalButton)
    {
        float axisHorizontal = (
            this.globalCtrl.directionSwitch ? Input.GetAxis(verticalButton) : Input.GetAxis(horizontalButton)
        );     
        float axisVertical = (
            this.globalCtrl.directionSwitch ? Input.GetAxis(horizontalButton) : Input.GetAxis(verticalButton)
        );

        if (this.globalCtrl.directionHorizontalSignSwitch) {
            axisHorizontal *= -1;
        }
        if (this.globalCtrl.directionVerticalSignSwitch) {
            axisVertical *= -1;
        }

        Vector3 direction = new Vector3(axisVertical, 0, axisHorizontal);

        if (direction != Vector3.zero)
        {
            this.characterAnimator.SetFloat(this.speedParamHash, this.animationSpeed * 1F);
            this.navMeshAgent.Move(direction * this.animationSpeed * Time.deltaTime);

            this.characterTransform.LookAt(this.characterTransform.position + direction);
        }
        else
        {
            this.characterAnimator.SetFloat(this.speedParamHash, 0F);
        }
    }

    private void Update()
    {
        if (!this.globalCtrl.sceneCtrl.sceneState.isDungeonScene && this.navMeshAgent != null)
        {
            this.MovePlayer("Left Stick Horizontal", "Left Stick Vertical");
        }
    }
}
