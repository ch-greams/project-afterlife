using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class PlayerActionManager : IWithEndOfTurnAction
{
    [BoxGroup("[B] Walk Button")]
    public Button walkButton;
    [BoxGroup("[B] Walk Button")]
    public Sprite walkButtonActive;
    [BoxGroup("[B] Walk Button")]
    public Sprite walkButtonInactive;

    [BoxGroup("[X] Flashlight Button")]
    public Button flashlightButton;
    [BoxGroup("[X] Flashlight Button")]
    public Sprite flashlightButtonActive;
    [BoxGroup("[X] Flashlight Button")]
    public Sprite flashlightButtonInactive;

    [BoxGroup("[A] Torch Button")]
    public Button torchButton;
    [BoxGroup("[A] Torch Button")]
    public Sprite torchButtonActive;
    [BoxGroup("[A] Torch Button")]
    public Sprite torchButtonInactive;

    [BoxGroup("[Y] Granade Button")]
    public Button granadeButton;
    [BoxGroup("[Y] Granade Button")]
    public Sprite granadeButtonActive;
    [BoxGroup("[Y] Granade Button")]
    public Sprite granadeButtonInactive;


    public PlayerActionType currentAction;

    public List<EndOfTurnAction> endOfTurnActions { get; set; }

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
    }


    public IEnumerator InputListener()
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
            Debug.Log("Button Y");
            // GranadeButton
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
                case PlayerActionType.Torch:
                case PlayerActionType.Granade:
                case PlayerActionType.Undefined:
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
        Player player = this.globalCtrl.sceneCtrl.player;

        player.characterTransform.LookAt(this.selectedTile.obj.transform.position);
        player.flashlightRay.SetActive(true);

        player.UseFlashlight(this.selectedTile.point);

        yield return this.FLASHLIGHT_TIMEOUT;
        player.flashlightRay.SetActive(false);
    }

    public void ConfirmAction()
    {
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
            case PlayerActionType.Torch:
            case PlayerActionType.Granade:
            case PlayerActionType.Undefined:
            default:
                yield return null;
                break;
        }
    
        this.SelectActionType(PlayerActionType.Undefined);
    }

    private void DisableActionButton(PlayerActionType playerActionType)
    {
        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                this.walkButton.image.sprite = this.walkButtonInactive;
                this.walkButton.onClick.RemoveAllListeners();
                this.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
                break;
            case PlayerActionType.Flashlight:
                this.flashlightButton.image.sprite = this.flashlightButtonInactive;
                this.flashlightButton.onClick.RemoveAllListeners();
                this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
                break;
            case PlayerActionType.Undefined:
            default:
                break;
        }
    }

    private void SelectActionType(PlayerActionType playerActionType)
    {
        // Debug.Log("Selected Action - " + playerActionType.ToString());
        PlayerActionType prevAction = this.currentAction;
        this.currentAction = playerActionType;
        Player player = this.globalCtrl.sceneCtrl.player;

        // TODO: Shouldn't be used like this
        player.HighlightVisible(true);

        if (prevAction != this.currentAction) {
            this.DisableActionButton(prevAction);
        }

        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                this.walkButton.image.sprite = this.walkButtonActive;
                this.walkButton.onClick.RemoveAllListeners();
                this.walkButton.onClick.AddListener(this.ConfirmAction);

                player.HighlightActive(true, true);

                break;
            case PlayerActionType.Flashlight:
                this.flashlightButton.image.sprite = this.flashlightButtonActive;
                this.flashlightButton.onClick.RemoveAllListeners();
                this.flashlightButton.onClick.AddListener(this.ConfirmAction);
                
                player.HighlightActive(false, true);
                player.HighlightActive(true, false);

                break;
            case PlayerActionType.Undefined:
            default:
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
    Torch,
    Granade,
}
