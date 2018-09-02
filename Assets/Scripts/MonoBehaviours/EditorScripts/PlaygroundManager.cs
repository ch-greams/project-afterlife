using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PlaygroundManager : MonoBehaviour
{
    public Button walkButton;
    public Button flashlightButton;
    public Button confirmButton;

    public GameObject walkLabel;
    public GameObject flashlightLabel;

    public ActionTypeState currentAction;

    public GlobalController globalCtrl;


    private Tile selectedTile;
    private bool axisButtonInUse = false;


    private void Update()
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
                case ActionTypeState.Walk:
                    this.StartCoroutine(this.OnAxisButtonUse_ForWalk());
                    break;
                case ActionTypeState.Flashlight:
                    this.StartCoroutine(this.OnAxisButtonUse_ForDirection());
                    break;
                case ActionTypeState.Sword:
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
                this.selectedTile.obj.GetComponent<Interactable>().OnPointerExit(null);
            }

            this.selectedTile = nextTile;
            this.selectedTile.obj.GetComponent<Interactable>().OnPointerEnter(null);

            this.axisButtonInUse = true;
            yield return new WaitForSeconds(0.2F);
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
                    ? this.selectedTile.GetTileByDirection(direction)
                    : this.globalCtrl.sceneCtrl.player.tile.GetTileByDirection(direction)
            );

            yield return this.SelectNextTile(nextTile);
        }
    }

    private IEnumerator OnAxisButtonUse_ForDirection()
    {
        TileDirection direction = this.GetDirectionFromAxis("Left Stick Horizontal", "Left Stick Vertical");

        if (direction != TileDirection.Undefined)
        {
            Tile nextTile = this.globalCtrl.sceneCtrl.player.tile.GetTileByDirection(direction);

            yield return this.SelectNextTile(nextTile);
        }
    }



    public void Walk()
    {
        Debug.Log("Walk Button Triggered - Select Tile");

        this.currentAction = ActionTypeState.Walk;

        this.walkLabel.SetActive(true);
        this.flashlightLabel.SetActive(false);
    }

    private void WalkAction()
    {
        if (this.selectedTile)
        {
            this.selectedTile.obj.GetComponent<Interactable>().OnPointerClick(null);
        }
    }

    public void UseFlashlight()
    {
        Debug.Log("Flashlight Button Triggered - Select Direction");

        this.currentAction = ActionTypeState.Flashlight;

        this.walkLabel.SetActive(false);
        this.flashlightLabel.SetActive(true);

        // Highlight TILES/ENEMIES in the range from selected direction
    }

    private IEnumerator UseFlashlightAction()
    {
        Debug.Log("Flashlight use confirmed");

        Player player = this.globalCtrl.sceneCtrl.player;
        EnemyManager enemyManager = this.globalCtrl.enemyManager;

        player.characterTransform.LookAt(this.selectedTile.obj.transform.position);
        player.flashlightRay.SetActive(true);

        List<Tile> rayPath = player.tile.GetTilesByDirection(this.selectedTile.point, player.visibleRange);

        foreach (Tile tile in rayPath)
        {
            // Debug.Log("step [" + rayPath.IndexOf(tile) + "] = " + tile.point);

            Enemy enemy = enemyManager.enemies.Find((e) => e.tile.point == tile.point);

            if (enemy != null) {
                GameObject.DestroyImmediate(enemy.characterTransform.gameObject);
                enemy.tile.isBlocked = false;
                enemyManager.enemies.Remove(enemy);
            }
        }

        yield return new WaitForSeconds(0.25F);
        player.flashlightRay.SetActive(false);
    }


    public void ConfirmAction()
    {
        switch (this.currentAction)
        {
            case ActionTypeState.Walk:
                this.WalkAction();
                break;
            case ActionTypeState.Flashlight:
                base.StartCoroutine(this.UseFlashlightAction());
                break;
        }

        this.globalCtrl.enemyManager.NextTurn();
    }
}

public enum ActionTypeState
{
    Undefined,
    Walk,
    Flashlight,
    Sword,
    // Bomb,
}
