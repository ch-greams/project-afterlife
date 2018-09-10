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
            // SwordButton
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

    private IEnumerator SelectNextTile(Tile nextTile)
    {
        if (nextTile)
        {
            if (this.selectedTile)
            {
                Renderer _tileRenderer = this.selectedTile.obj.GetComponent<Interactable>().data.renderer;
                _tileRenderer.material.SetColor("_Color", new Color(0, 0, 0, 0.39F));
                // this.selectedTile.obj.GetComponent<Interactable>().OnPointerExit(null);
            }

            this.selectedTile = nextTile;

            Renderer tileRenderer = this.selectedTile.obj.GetComponent<Interactable>().data.renderer;
            tileRenderer.material.SetColor("_Color", Color.cyan);
            // this.selectedTile.obj.GetComponent<Interactable>().OnPointerEnter(null);

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
            Tile nextTile = (
                this.selectedTile
                    ? this.selectedTile.GetTileByDirection(direction, (t) => (!t.isBlocked && t.isVisible))
                    : this.globalCtrl.sceneCtrl.player.tile.GetTileByDirection(direction, (t) => (!t.isBlocked && t.isVisible))
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
            Tile nextTile = playerTile.GetTileByDirection(direction, (t) => (t.isVisible));

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
            enemyManager.TryKillEnemyOnPoint(tile.point);
        }

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
            case PlayerActionType.Undefined:
            default:
                yield return null;
                break;
        }
    
        this.SelectActionType(PlayerActionType.Undefined);
    }

    private void SelectActionType(PlayerActionType playerActionType)
    {
        // Debug.Log("Selected Action - " + playerActionType.ToString());
        this.currentAction = playerActionType;

        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                this.walkLabel.SetActive(true);
                this.flashlightLabel.SetActive(false);
                break;
            case PlayerActionType.Flashlight:
                this.walkLabel.SetActive(false);
                this.flashlightLabel.SetActive(true);
                break;
            case PlayerActionType.Undefined:
            default:
                this.walkLabel.SetActive(false);
                this.flashlightLabel.SetActive(false);
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
