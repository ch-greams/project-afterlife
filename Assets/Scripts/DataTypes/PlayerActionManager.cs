using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class PlayerActionManager
{
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[B] Walk Button")]
    public Button walkButton;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonActive;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonInactive;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[B] Walk Button")]
    public Image walkButtonProc;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonProcActive;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonProcInactive;


    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[X] Flashlight Button")]
    public Button flashlightButton;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[X] Flashlight Button")]
    public Sprite flashlightButtonActive;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[X] Flashlight Button")]
    public Sprite flashlightButtonInactive;

    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[A] Torch Button")]
    public Button torchButton;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[A] Torch Button")]
    public Sprite torchButtonActive;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[A] Torch Button")]
    public Sprite torchButtonInactive;

    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[Y] Granade Button")]
    public Button granadeButton;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[Y] Granade Button")]
    public Sprite granadeButtonActive;
    [FoldoutGroup("Button Configuration")]
    [BoxGroup("Button Configuration/[Y] Granade Button")]
    public Sprite granadeButtonInactive;

    public GameObject enemyTurnFadeImage;


    public bool arePlayerControlsLocked = false;
    public float moveSpeed = 5;
    public float maxDistanceForSelector = 7;


    public PlayerActionType currentAction { get; private set; }
    public Tile selectedTile { get; private set; }
    public HashSet<Tile> selectedTiles { get; private set; }

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        this.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
        this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
        this.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
    }


    public void InputListener()
    {
        if (!this.arePlayerControlsLocked)
        {
            if (Input.GetButtonDown("Button A"))
            {
                Debug.Log("Button A");
                // TorchButton
            }

            if (Input.GetButtonDown("Button B"))
            {
                // Debug.Log("Button B");
                this.walkButton.onClick.Invoke();
            }

            if (Input.GetButtonDown("Button X"))
            {
                // Debug.Log("Button X");
                this.flashlightButton.onClick.Invoke();
            }

            if (Input.GetButtonDown("Button Y"))
            {
                // Debug.Log("Button Y");
                this.granadeButton.onClick.Invoke();
            }

            if (Input.GetButtonDown("Left Stick Button"))
            {
                // Debug.Log("Left Stick Button");
                this.ResetTileSelector(true);
            } 

            switch (this.currentAction)
            {
                case PlayerActionType.Walk:
                case PlayerActionType.Flashlight:
                case PlayerActionType.Granade:
                    this.TryMoveTileSelector("Left Stick Horizontal", "Left Stick Vertical");
                    break;
                case PlayerActionType.Torch:
                case PlayerActionType.Undefined:
                default:
                    break;
            }
        }
    }

    public void ConfirmAction()
    {
        if (this.selectedTile)
        {
            this.ResetTileSelector(false);

            this.globalCtrl.NextTurn();
        }
    }

    public void SwitchWalkProcEffect(bool active)
    {
        this.walkButtonProc.gameObject.SetActive(active);
        
        this.flashlightButton.interactable = !active;
        this.granadeButton.interactable = !active;

        if (active)
        {
            this.flashlightButton.onClick.RemoveAllListeners();
            this.granadeButton.onClick.RemoveAllListeners();
        }
        else
        {
            this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
            this.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
        }
    }


    public void SelectActionType(PlayerActionType playerActionType)
    {
        PlayerActionType prevAction = this.currentAction;
        this.currentAction = playerActionType;
        Player player = this.globalCtrl.sceneCtrl.player;

        if (prevAction != this.currentAction)
        {
            this.DisableActionButton(prevAction);

            switch (playerActionType)
            {
                case PlayerActionType.Walk:
                    this.walkButton.image.sprite = this.walkButtonActive;
                    this.walkButtonProc.sprite = this.walkButtonProcActive;
                    this.walkButton.onClick.RemoveAllListeners();
                    this.walkButton.onClick.AddListener(this.ConfirmAction);

                    player.HighlightActive(true, true, (t) => (!t.isBlocked));
                    break;
                case PlayerActionType.Flashlight:
                    this.flashlightButton.image.sprite = this.flashlightButtonActive;
                    this.flashlightButton.onClick.RemoveAllListeners();
                    this.flashlightButton.onClick.AddListener(this.ConfirmAction);

                    player.HighlightActive(false, true, (t) => (true));
                    player.HighlightActive(true, false, (t) => (true));
                    break;
                case PlayerActionType.Granade:
                    this.granadeButton.image.sprite = this.granadeButtonActive;
                    this.granadeButton.onClick.RemoveAllListeners();
                    this.granadeButton.onClick.AddListener(this.ConfirmAction);
                    
                    player.HighlightActive(true, true, (t) => (true));
                    break;
                case PlayerActionType.Torch:
                case PlayerActionType.Undefined:
                default:
                    player.HighlightActive(false, true, (t) => (true));
                    break;
            }
        }
    }

    private void DisableActionButton(PlayerActionType playerActionType)
    {
        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                this.walkButton.image.sprite = this.walkButtonInactive;
                this.walkButtonProc.sprite = this.walkButtonProcInactive;
                this.walkButton.onClick.RemoveAllListeners();
                this.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
                break;
            case PlayerActionType.Flashlight:
                this.flashlightButton.image.sprite = this.flashlightButtonInactive;
                this.flashlightButton.onClick.RemoveAllListeners();
                this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
                break;
            case PlayerActionType.Granade:
                this.granadeButton.image.sprite = this.granadeButtonInactive;
                this.granadeButton.onClick.RemoveAllListeners();
                this.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
                break;
            case PlayerActionType.Torch:
            case PlayerActionType.Undefined:
            default:
                break;
        }
    }


    private void ResetTileSelector(bool selectPlayerTile)
    {
        Player player = this.globalCtrl.sceneCtrl.player;
        Transform tileSelector = player.tileSelector.transform;
        Vector3 playerPosition = player.characterTransform.position;
        tileSelector.position = new Vector3(playerPosition.x, tileSelector.position.y, playerPosition.z);

        if (selectPlayerTile)
        {
            this.SelectTile(player.tile);
        }
    }

    private void SelectTile(Tile tile)
    {
        if (tile)
        {
            if (this.selectedTile)
            {
                this.selectedTile.RefreshTileState(
                    this.selectedTile.isVisible, this.selectedTile.isBlocked,
                    this.selectedTile.isActive, false
                );
            }

            this.selectedTile = tile;

            this.selectedTile.RefreshTileState(
                this.selectedTile.isVisible, this.selectedTile.isBlocked,
                this.selectedTile.isActive, true
            );
        }
    }

    private HashSet<Tile> GetTilesPerActionType(Tile selectedTile)
    {
        Player player = this.globalCtrl.sceneCtrl.player;
        HashSet<Tile> selectedTiles = new HashSet<Tile>();
        selectedTiles.Add(selectedTile);

        switch (this.currentAction)
        {
            case PlayerActionType.Flashlight:
                selectedTiles.UnionWith(
                    player.tile.GetTilesByDirection(selectedTile.point, player.flashlightRange)
                );
                break;
            case PlayerActionType.Granade:
                selectedTiles.UnionWith(
                    selectedTile.GetTilesInRange(1, (t) => (true))
                );
                break;
            case PlayerActionType.Walk:
            case PlayerActionType.Torch:
            case PlayerActionType.Undefined:
            default:
                break;
        }

        return selectedTiles;
    }

    // TODO: Make private later
    public void UpdateSelectedTiles(HashSet<Tile> tiles)
    {
        if (tiles != null)
        {
            if (this.selectedTiles != null)
            {
                foreach (Tile tile in this.selectedTiles)
                {
                    tile.RefreshTileState(tile.isVisible, tile.isBlocked, tile.isActive, false);
                }
            }

            this.selectedTiles = tiles;

            foreach (Tile tile in this.selectedTiles)
            {
                tile.RefreshTileState(tile.isVisible, tile.isBlocked, tile.isActive, true);
            }
        }
    }

    private void TrySelectTile(Vector3 target)
    {
        Player player = this.globalCtrl.sceneCtrl.player;

        if ((player.activeTiles != null) && (player.activeTiles.Count > 0))
        {
            List<Vector3> positions = player.activeTiles.Keys.ToList();
            positions.Sort((pos1, pos2) => Vector3.Distance(target, pos1).CompareTo(Vector3.Distance(target, pos2)));

            Tile closestTile = player.activeTiles[positions.First()];

            this.SelectTile(closestTile);

            this.UpdateSelectedTiles(this.GetTilesPerActionType(closestTile));
        }
    }

    private void TryMoveTileSelector(string horizontalButton, string verticalButton)
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
        Vector3 translation = direction * this.moveSpeed * Time.deltaTime;

        if (translation != Vector3.zero)
        {
            Player player = this.globalCtrl.sceneCtrl.player;
            Vector3 playerPosition = player.characterTransform.position;
            Vector3 target = player.tileSelector.transform.position + translation;

            if (Vector3.Distance(playerPosition, target) < this.maxDistanceForSelector)
            {
                player.tileSelector.transform.Translate(translation, Space.World);

                this.TrySelectTile(target);
            }
        }
    }
}

public enum PlayerActionType
{
    Undefined,
    Walk,
    Flashlight,
    Torch,
    Granade,
}
