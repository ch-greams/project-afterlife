using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using System;


public class PlayerActionManager : SerializedMonoBehaviour
{
    public PlayerActionInterface interfaceElements;

    public List<PlayerActionType> instantActions = new List<PlayerActionType>();

    // TODO: Update boxGroup below
    [BoxGroup("Other")]
    public GameObject enemyTurnFadeImage;
    [BoxGroup("Other")]
    public GameObject gameOverFade;
    

    public bool arePlayerControlsLocked = false;
    public float moveSpeed = 5;
    public float maxDistanceForSelector = 7;


    public Interactable currentInteractable { get; private set; }
    public PlayerActionType currentAction { get; private set; }
    public Tile selectedTile { get; private set; }
    public HashSet<Tile> selectedTiles { get; private set; }

    private GlobalController globalCtrl;
    private bool isXboxJoystick;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
        this.isXboxJoystick = this.IsXboxJoystick();

        this.interfaceElements.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
        this.interfaceElements.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
        this.interfaceElements.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
        this.interfaceElements.torchButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Torch));
        this.interfaceElements.skipTurnButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.SkipTurn));
        this.interfaceElements.interactionButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Interaction));
    }

    // TODO: Make something more sophisticated for controller switch
    // TODO: Probably replace with xInput or new Unity alternative later
    private bool IsXboxJoystick()
    {
        return Array.Exists(
            Input.GetJoystickNames(),
            (joystick) => (joystick != null) && joystick.ToLower().Contains("xbox")
        );
    }

    public void InputListener(bool isDungeonScene)
    {
        if (isDungeonScene)
        {
            this.InputListener();
        }
        else
        {
            if (this.currentInteractable != null)
            {
                this.HoldButton(
                    buttonName: "Right Trigger",
                    button: this.interfaceElements.wInteractionButton,
                    progressTransform: this.interfaceElements.wInteractionButtonProgress,
                    progressCounter: ref this.interfaceElements.wInteractionButtonProgressCounter,
                    progressCounterMax: this.interfaceElements.wInteractionButtonProgressMax,
                    progressBarWidth: this.interfaceElements.wInteractionButton.GetComponent<RectTransform>().rect.width
                );
            }
        }
    }

    public void InputListener()
    {
        if (!this.arePlayerControlsLocked)
        {
            if (Input.GetButtonDown(this.isXboxJoystick ? "Button A" : "Button B"))
            {
                this.interfaceElements.torchButton.onClick.Invoke();
            }

            if (Input.GetButtonDown(this.isXboxJoystick ? "Button B" : "Button X"))
            {
                this.interfaceElements.walkButton.onClick.Invoke();
            }

            if (Input.GetButtonDown(this.isXboxJoystick ? "Button X" : "Button A"))
            {
                this.interfaceElements.flashlightButton.onClick.Invoke();
            }

            if (Input.GetButtonDown("Button Y"))
            {
                this.interfaceElements.granadeButton.onClick.Invoke();
            }

            if (this.currentInteractable != null)
            {
                this.HoldButton(
                    buttonName: "Right Trigger",
                    button: this.interfaceElements.interactionButton,
                    progressTransform: this.interfaceElements.interactionButtonProgress,
                    progressCounter: ref this.interfaceElements.interactionButtonProgressCounter,
                    progressCounterMax: this.interfaceElements.interactionButtonProgressMax,
                    // TODO: Remove this comment and/or optimize (replaced hardcoded 200)
                    progressBarWidth: this.interfaceElements.interactionButton.GetComponent<RectTransform>().rect.width
                );
            }

            this.HoldButton(
                buttonName: "Left Trigger",
                button: this.interfaceElements.skipTurnButton,
                progressTransform: this.interfaceElements.skipTurnButtonProgress,
                progressCounter: ref this.interfaceElements.skipTurnButtonProgressCounter,
                progressCounterMax: this.interfaceElements.skipTurnButtonProgressMax,
                // TODO: Remove this comment and/or optimize (replaced hardcoded 300)
                progressBarWidth: this.interfaceElements.skipTurnButton.GetComponent<RectTransform>().rect.width
            );

            if (Input.GetButtonDown("Left Stick Button"))
            {
                this.ResetTileSelector(true);
            } 

            switch (this.currentAction)
            {
                case PlayerActionType.Walk:
                case PlayerActionType.Flashlight:
                case PlayerActionType.Granade:
                case PlayerActionType.Torch:
                    this.TryMoveTileSelector("Left Stick Horizontal", "Left Stick Vertical");
                    break;
                case PlayerActionType.Interaction:
                case PlayerActionType.SkipTurn:
                case PlayerActionType.Undefined:
                default:
                    break;
            }
        }
    }

    private void HoldButton(
        string buttonName,
        Button button,
        RectTransform progressTransform,
        ref float progressCounter,
        float progressCounterMax,
        float progressBarWidth
    ) {
        float axisValue = Input.GetAxis(buttonName);

        if (axisValue > 0)
        {
            progressCounter += axisValue;

            this.UpdateProgressBar(
                progressBar: progressTransform,
                progress: progressCounter / progressCounterMax,
                progressBarWidth: progressBarWidth
            );

            if (progressCounter >= progressCounterMax)
            {
                button.onClick.Invoke();

                progressCounter = 0;
                this.UpdateProgressBar(
                    progressBar: progressTransform,
                    progress: 0,
                    progressBarWidth: progressBarWidth
                );
            }
        }
        else
        {
            progressCounter = 0;
            this.UpdateProgressBar(
                progressBar: progressTransform,
                progress: 0,
                progressBarWidth: progressBarWidth
            );
        }
    }

    private void UpdateProgressBar(RectTransform progressBar, float progress, float progressBarWidth)
    {
        progress = progress > 1 ? 1 : progress;

        float sizeDeltaX = (progressBarWidth * progress) - progressBarWidth;
        progressBar.sizeDelta = new Vector2(sizeDeltaX, progressBar.sizeDelta.y);

        float anchoredPositionX = ((progressBarWidth * progress) / 2) - (progressBarWidth / 2);
        progressBar.anchoredPosition = new Vector2(anchoredPositionX, progressBar.anchoredPosition.y);
    }

    public void ConfirmAction()
    {
        if (this.selectedTile)
        {
            this.ResetTileSelector(false);

            this.globalCtrl.NextTurn();
        }
    }

    public void SwitchWalkProcEffect(bool isProcActive)
    {
        this.interfaceElements.walkButtonProc.gameObject.SetActive(isProcActive);
        
        this.interfaceElements.flashlightButton.interactable = !isProcActive;
        this.interfaceElements.torchButton.interactable = !isProcActive;

        if (isProcActive)
        {
            this.interfaceElements.flashlightButton.onClick.RemoveAllListeners();
            this.interfaceElements.torchButton.onClick.RemoveAllListeners();
        }
        else
        {
            this.interfaceElements.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
            this.interfaceElements.torchButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Torch));
        }

        if (isProcActive)
        {
            this.interfaceElements.granadeButton.interactable = false;
            this.interfaceElements.granadeButton.onClick.RemoveAllListeners();
        }
        else if (this.globalCtrl.globalState.GetIntegerParameterFromState("turnsTillGranadeChargeLeft") < 1)
        {
            this.interfaceElements.granadeButton.interactable = true;
            this.interfaceElements.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
        }
    }


    public void GranadeChargeEffect(int turnsTillChargeLeft)
    {
        if (turnsTillChargeLeft > 0)
        {
            this.interfaceElements.granadeButton.interactable = false;
            this.interfaceElements.granadeButton.onClick.RemoveAllListeners();

            this.interfaceElements.granadeButtonCooldownLabel.text = turnsTillChargeLeft.ToString();
            this.interfaceElements.granadeButtonCooldownLabel.gameObject.SetActive(true);
        }
        else
        {
            this.interfaceElements.granadeButton.interactable = true;
            this.interfaceElements.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));

            this.interfaceElements.granadeButtonCooldownLabel.gameObject.SetActive(false);
        }
    }

    public void SelectActionType(PlayerActionType playerActionType)
    {
        PlayerActionType prevAction = this.currentAction;
        this.currentAction = playerActionType;
        Player player = this.globalCtrl.sceneCtrl.player;

        if (this.instantActions.Contains(this.currentAction) || prevAction != this.currentAction)
        {
            this.DisableActionButton(prevAction);

            this.SelectTile(null);
            this.UpdateSelectedTiles(new HashSet<Tile>());

            this.ResetTileSelector(false);

            switch (playerActionType)
            {
                case PlayerActionType.Walk:
                    this.interfaceElements.walkButton.image.sprite = this.interfaceElements.walkButtonActive;
                    this.interfaceElements.walkButtonProc.sprite = this.interfaceElements.walkButtonProcActive;
                    this.interfaceElements.walkButton.onClick.RemoveAllListeners();
                    this.interfaceElements.walkButton.onClick.AddListener(this.ConfirmAction);

                    player.HighlightActive(true, true, (t) => (!t.isBlocked));
                    break;
                case PlayerActionType.Flashlight:
                    this.interfaceElements.flashlightButton.image.sprite = this.interfaceElements.flashlightButtonActive;
                    this.interfaceElements.flashlightButton.onClick.RemoveAllListeners();
                    this.interfaceElements.flashlightButton.onClick.AddListener(this.ConfirmAction);

                    player.HighlightActive(false, true, (t) => (true));
                    player.HighlightActive(true, false, (t) => (true));
                    break;
                case PlayerActionType.Granade:
                    this.interfaceElements.granadeButton.image.sprite = this.interfaceElements.granadeButtonActive;
                    this.interfaceElements.granadeButton.onClick.RemoveAllListeners();
                    this.interfaceElements.granadeButton.onClick.AddListener(this.ConfirmAction);
                    
                    player.HighlightActive(true, true, (t) => (true));
                    break;
                case PlayerActionType.Torch:
                    this.interfaceElements.torchButton.image.sprite = this.interfaceElements.torchButtonActive;
                    this.interfaceElements.torchButton.onClick.RemoveAllListeners();
                    this.interfaceElements.torchButton.onClick.AddListener(this.ConfirmAction);

                    player.HighlightActive(false, true, (t) => (true));
                    player.HighlightActive(true, false, (t) => (true));
                    break;
                case PlayerActionType.Interaction:
                case PlayerActionType.SkipTurn:
                    this.globalCtrl.NextTurn();
                    break;
                case PlayerActionType.Undefined:
                default:
                    player.HighlightActive(false, true, (t) => (true));
                    break;
            }
        }
    }

    public void SelectInteractable(Interactable interactable, bool isDungeonScene)
    {
        this.currentInteractable = interactable;

        if (isDungeonScene)
        {
            this.interfaceElements.interactionButton.gameObject.SetActive(true);
        }
        else
        {
            this.interfaceElements.wInteractionButtonLabel.text = string.Format("[RT] {0}", (
                string.IsNullOrWhiteSpace(this.currentInteractable.data.actionLabel)
                    ? "Use"
                    : this.currentInteractable.data.actionLabel
            ));

            this.interfaceElements.wInteractionButton.onClick.AddListener(this.currentInteractable.OnClickSync);
            this.interfaceElements.wInteractionButton.gameObject.SetActive(true);
        }
    }

    public void DeselectInteractable(bool isDungeonScene)
    {
        this.currentInteractable = null;

        if (isDungeonScene)
        {
            this.interfaceElements.interactionButton.gameObject.SetActive(false);
        }
        else
        {
            this.interfaceElements.wInteractionButton.onClick.RemoveAllListeners();
            this.interfaceElements.wInteractionButton.gameObject.SetActive(false);
        }
    }

    private void DisableActionButton(PlayerActionType playerActionType)
    {
        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                this.interfaceElements.walkButton.image.sprite = this.interfaceElements.walkButtonInactive;
                this.interfaceElements.walkButtonProc.sprite = this.interfaceElements.walkButtonProcInactive;
                this.interfaceElements.walkButton.onClick.RemoveAllListeners();
                this.interfaceElements.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
                break;
            case PlayerActionType.Flashlight:
                this.interfaceElements.flashlightButton.image.sprite = this.interfaceElements.flashlightButtonInactive;
                this.interfaceElements.flashlightButton.onClick.RemoveAllListeners();
                this.interfaceElements.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
                break;
            case PlayerActionType.Granade:
                this.interfaceElements.granadeButton.image.sprite = this.interfaceElements.granadeButtonInactive;
                this.interfaceElements.granadeButton.onClick.RemoveAllListeners();
                this.interfaceElements.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
                break;
            case PlayerActionType.Torch:
                this.interfaceElements.torchButton.image.sprite = this.interfaceElements.torchButtonInactive;
                this.interfaceElements.torchButton.onClick.RemoveAllListeners();
                this.interfaceElements.torchButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Torch));
                break;
            case PlayerActionType.Interaction:
            case PlayerActionType.SkipTurn:
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
        if (this.selectedTile)
        {
            this.selectedTile.RefreshTileState(
                this.selectedTile.isVisible, this.selectedTile.isBlocked,
                this.selectedTile.isActive, false
            );
        }

        this.selectedTile = tile;

        if (this.selectedTile != null)
        {
            this.selectedTile.RefreshTileState(
                this.selectedTile.isVisible, this.selectedTile.isBlocked,
                this.selectedTile.isActive, true
            );
        }
    }

    private HashSet<Tile> GetTilesPerActionType(Tile selectedTile)
    {
        Player player = this.globalCtrl.sceneCtrl.player;
        HashSet<Tile> selectedTiles = new HashSet<Tile>(){ selectedTile };

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
            case PlayerActionType.Torch:
                selectedTiles.UnionWith(
                    player.tile.GetCleaveTilesByDirection(selectedTile.point)
                );
                break;
            case PlayerActionType.Walk:
            case PlayerActionType.Interaction:
            case PlayerActionType.SkipTurn:
            case PlayerActionType.Undefined:
            default:
                break;
        }

        return selectedTiles;
    }

    // TODO: Make private later
    public void UpdateSelectedTiles(HashSet<Tile> tiles)
    {
        if (this.selectedTiles != null)
        {
            foreach (Tile tile in this.selectedTiles)
            {
                tile.RefreshTileState(tile.isVisible, tile.isBlocked, tile.isActive, false);
            }
        }

        this.selectedTiles = tiles;

        if (this.selectedTiles != null)
        {
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
    Interaction,
    SkipTurn,
}
