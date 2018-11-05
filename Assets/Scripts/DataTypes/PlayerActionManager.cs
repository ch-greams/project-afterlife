using System.Linq;
using System.Collections;
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

    public PlayerActionType currentAction;

    public float moveSpeed = 5;
    public float maxDistanceForSelector = 7;

    private GlobalController globalCtrl;
    private Tile selectedTile;
    private WaitForSeconds FLASHLIGHT_TIMEOUT;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
        this.FLASHLIGHT_TIMEOUT = new WaitForSeconds(0.25F);

        this.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
        this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
    }


    public void InputListener()
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

        if (Input.GetButtonDown("Left Stick Button"))
        {
            // Debug.Log("Left Stick Button");
            this.ResetTileSelector(true);
        } 

        switch (this.currentAction)
        {
            case PlayerActionType.Walk:
            case PlayerActionType.Flashlight:
                this.TryMoveTileSelector("Left Stick Horizontal", "Left Stick Vertical");
                break;
            case PlayerActionType.Torch:
            case PlayerActionType.Granade:
            case PlayerActionType.Undefined:
            default:
                break;
        }
    }

    public void ConfirmAction()
    {
        this.ResetTileSelector(false);

        this.globalCtrl.NextTurn();
    }

    public IEnumerator TriggerSelectedAction()
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

    public void SwitchWalkProcEffect(bool active)
    {
        this.walkButtonProc.gameObject.SetActive(active);
    }


    private void ResetTileSelector(bool selectPlayerTile)
    {
        Player player = this.globalCtrl.sceneCtrl.player;
        Transform tileSelector = player.tileSelector.transform;
        Vector3 playerPosition = player.characterTransform.position;
        tileSelector.position = new Vector3(playerPosition.x, tileSelector.position.y, playerPosition.z);

        if (selectPlayerTile)
        {
            this.SelectNextTile(player.tile);
        }
    }


    private void TrySelectTile(Player player, Vector3 target)
    {
        if ((player.activeTiles != null) && (player.activeTiles.Count > 0))
        {
            List<Vector3> positions = player.activeTiles.Keys.ToList();
            positions.Sort((pos1, pos2) => Vector3.Distance(target, pos1).CompareTo(Vector3.Distance(target, pos2)));

            Tile closestTile = player.activeTiles[positions.First()];

            this.SelectNextTile(closestTile);
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

                this.TrySelectTile(player, target);
            }
        }
    }

    private void SelectNextTile(Tile nextTile)
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
            case PlayerActionType.Torch:
            case PlayerActionType.Granade:
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

                    player.HighlightActive(true, true);

                    break;
                case PlayerActionType.Flashlight:
                    this.flashlightButton.image.sprite = this.flashlightButtonActive;
                    this.flashlightButton.onClick.RemoveAllListeners();
                    this.flashlightButton.onClick.AddListener(this.ConfirmAction);
                    
                    player.HighlightActive(false, true);
                    player.HighlightActive(true, false);

                    break;
                case PlayerActionType.Torch:
                case PlayerActionType.Granade:
                case PlayerActionType.Undefined:
                default:
                    player.HighlightActive(false, true);
                    break;
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
