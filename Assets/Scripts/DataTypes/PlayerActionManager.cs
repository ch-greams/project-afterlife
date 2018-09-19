using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerActionManager
{
    public Button walkButton;
    public Button flashlightButton;
    public Button confirmButton;

    public GameObject walkLabel;
    public GameObject flashlightLabel;

    public PlayerActionType currentAction;


    private GlobalController globalCtrl;
    private Tile selectedTile;
    private bool axisButtonInUse = false;
    private WaitForSeconds FLASHLIGHT_TIMEOUT;
    private WaitForSeconds SELECT_TILE_TIMEOUT;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
        this.FLASHLIGHT_TIMEOUT = new WaitForSeconds(0.25F);
        this.SELECT_TILE_TIMEOUT = new WaitForSeconds(0.15F);

        this.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
        this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));

        this.confirmButton.onClick.AddListener(this.ConfirmAction);
    }


    public IEnumerator InputListener()
    {
        if (Input.GetButtonDown("Button A"))
        {
            // Debug.Log("Button A");
            this.confirmButton.onClick.Invoke();
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
            Debug.Log("Button Y");
            // TorchButton
        }

        if (!axisButtonInUse)
        {
            switch (this.currentAction)
            {
                case PlayerActionType.Walk:
                    yield return this.OnAxisButtonUse_ForWalk();
                    break;
                case PlayerActionType.Flashlight:
                    yield return this.OnAxisButtonUse_ForDirection();
                    break;
                case PlayerActionType.Sword:
                default:
                    break;
            }
        }
    }


    private TileDirection GetDirectionFromAxis(string horizontalButton, string verticalButton)
    {
        const float BUFFER = 0.2F;
        
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


        if (axisHorizontal > BUFFER)
        {
            if (axisVertical > BUFFER)
            {
                return TileDirection.UpRight;
            }
            else if (axisVertical < (-1 * BUFFER))
            {
                return TileDirection.DownRight;
            }
            else
            {
                return TileDirection.Right;
            }
        }
        else if (axisHorizontal < (-1 * BUFFER))
        {
            if (axisVertical > BUFFER)
            {
                return TileDirection.UpLeft;
            }
            else if (axisVertical < (-1 * BUFFER))
            {
                return TileDirection.DownLeft;
            }
            else
            {
                return TileDirection.Left;
            }
        }
        else
        {
            if (axisVertical > BUFFER)
            {
                return TileDirection.Up;
            }
            else if (axisVertical < (-1 * BUFFER))
            {
                return TileDirection.Down;
            }
            else
            {
                return TileDirection.Undefined;
            }
        }
    }

    // TODO: Add animation
    private IEnumerator SelectNextTile(Tile nextTile)
    {
        if (nextTile)
        {
            if (this.selectedTile)
            {
                this.selectedTile.RefreshTileState(
                    this.selectedTile.isVisible, this.selectedTile.isBlocked,
                    this.selectedTile.isActive, false
                );
            }

            this.selectedTile = nextTile;

            this.selectedTile.RefreshTileState(
                this.selectedTile.isVisible, this.selectedTile.isBlocked,
                this.selectedTile.isActive, true
            );

            this.axisButtonInUse = true;
            yield return this.SELECT_TILE_TIMEOUT;
            this.axisButtonInUse = false;
        }
    }

    private IEnumerator OnAxisButtonUse_ForWalk()
    {
        TileDirection direction = this.GetDirectionFromAxis("Left Stick Horizontal", "Left Stick Vertical");

        if (direction != TileDirection.Undefined)
        {
            Tile playerTile = this.globalCtrl.sceneCtrl.player.tile;
            Func<Tile, bool> filter = (t) => (!t.isBlocked && t.isVisible && t.isActive);

            Tile nextTile = (
                this.selectedTile
                    ? this.selectedTile.GetTileByDirection(direction, filter)
                    : playerTile.GetTileByDirection(direction, filter)
            );

            yield return this.SelectNextTile(nextTile);
        }
    }

    private IEnumerator OnAxisButtonUse_ForDirection()
    {
        TileDirection direction = this.GetDirectionFromAxis("Left Stick Horizontal", "Left Stick Vertical");

        if (direction != TileDirection.Undefined)
        {
            Tile playerTile = this.globalCtrl.sceneCtrl.player.tile;

            Tile nextTile = (
                this.selectedTile
                    ? this.selectedTile.GetTileByDirection(direction, (t) => (t.isVisible && t.isActive))
                    : playerTile.GetTileByDirection(direction, (t) => (t.isVisible && t.isActive))
            );

            yield return this.SelectNextTile(nextTile);
        }
    }

    private IEnumerator WalkAction()
    {
        if (this.selectedTile)
        {
            yield return this.globalCtrl.sceneCtrl.player.MoveToTile(this.selectedTile);
        }
    }

    // TODO: Make this prettier
    private IEnumerator UseFlashlightAction()
    {
        // Debug.Log("Flashlight use confirmed");

        Player player = this.globalCtrl.sceneCtrl.player;
        EnemyManager enemyManager = this.globalCtrl.enemyManager;

        player.characterTransform.LookAt(this.selectedTile.obj.transform.position);
        player.flashlightRay.SetActive(true);

        Path<Tile> rayPath = player.tile.GetTilesByDirection(this.selectedTile.point, player.flashlightRange);
        foreach (Tile tile in rayPath)
        {
            if (enemyManager.TryDestroyEnemyOnPoint(tile.point))
            {
                // TODO: Move to appropriate place
                this.globalCtrl.collectableManager.TrySpawnItem(tile.point, "Healthpack");
            }
        }

        yield return this.FLASHLIGHT_TIMEOUT;
        player.flashlightRay.SetActive(false);
    }

    public void ConfirmAction()
    {
        this.globalCtrl.StartCoroutine(this.ButtonHighlight(this.confirmButton));

        this.globalCtrl.NextTurn();
    }

    public IEnumerator OnTurnChange()
    {
        yield return this.TriggerSelectedAction();
    }

    private IEnumerator TriggerSelectedAction()
    {
        switch (this.currentAction)
        {
            case PlayerActionType.Walk:
                yield return this.WalkAction();
                break;
            case PlayerActionType.Flashlight:
                yield return this.UseFlashlightAction();
                break;
            case PlayerActionType.Undefined:
            default:
                yield return null;
                break;
        }
    
        this.SelectActionType(PlayerActionType.Undefined);
    }

    private IEnumerator ButtonHighlight(Button button)
    {
        ColorBlock colorBlock = button.colors;

        colorBlock.normalColor = new Color(0.5F, 1.0F, 0.5F, 1.0F);
        button.colors = colorBlock;

        yield return new WaitForSeconds(0.25F);

        colorBlock.normalColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        button.colors = colorBlock;
    }

    private void ButtonSelectHighlight(Button button, bool selected)
    {
        ColorBlock colorBlock = button.colors;

        colorBlock.normalColor = (
            selected ? new Color(0.5F, 1.0F, 0.5F, 1.0F) : new Color(1.0F, 1.0F, 1.0F, 1.0F)
        );
        button.colors = colorBlock;
    }

    private void SelectActionType(PlayerActionType playerActionType)
    {
        // Debug.Log("Selected Action - " + playerActionType.ToString());
        this.currentAction = playerActionType;
        Player player = this.globalCtrl.sceneCtrl.player;

        // TODO: Shouldn't be used like this
        player.HighlightVisible(true);

        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                this.walkLabel.SetActive(true);
                this.flashlightLabel.SetActive(false);

                this.ButtonSelectHighlight(this.walkButton, true);
                this.ButtonSelectHighlight(this.flashlightButton, false);

                player.HighlightActive(true, true);

                break;
            case PlayerActionType.Flashlight:
                this.walkLabel.SetActive(false);
                this.flashlightLabel.SetActive(true);

                this.ButtonSelectHighlight(this.walkButton, false);
                this.ButtonSelectHighlight(this.flashlightButton, true);
                
                player.HighlightActive(false, true);
                player.HighlightActive(true, false);

                break;
            case PlayerActionType.Undefined:
            default:
                this.walkLabel.SetActive(false);
                this.flashlightLabel.SetActive(false);

                this.ButtonSelectHighlight(this.walkButton, false);
                this.ButtonSelectHighlight(this.flashlightButton, false);
                
                player.HighlightActive(false, true);
                break;
        }
    }
}

public enum PlayerActionType
{
    Undefined,
    Walk,
    Flashlight,
    Sword,
    // Bomb,
}
