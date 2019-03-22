using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using System;


public class PlayerActionManager
{
    [BoxGroup("Group Configuration")]
    public GameObject playerActionsGroup;
    [BoxGroup("Group Configuration")]
    public GameObject statsPanelGroup;

    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[B] Walk Button")]
    public Button walkButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonActive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonInactive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[B] Walk Button")]
    public Image walkButtonProc;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonProcActive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[B] Walk Button")]
    public Sprite walkButtonProcInactive;


    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[X] Flashlight Button")]
    public Button flashlightButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[X] Flashlight Button")]
    public Sprite flashlightButtonActive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[X] Flashlight Button")]
    public Sprite flashlightButtonInactive;

    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[A] Torch Button")]
    public Button torchButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[A] Torch Button")]
    public Sprite torchButtonActive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[A] Torch Button")]
    public Sprite torchButtonInactive;

    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[Y] Granade Button")]
    public Button granadeButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[Y] Granade Button")]
    public Sprite granadeButtonActive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[Y] Granade Button")]
    public Sprite granadeButtonInactive;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[Y] Granade Button")]
    public Text granadeButtonCooldownLabel;

    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[LT] Skip Turn Button")]
    public Button skipTurnButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[LT] Skip Turn Button")]
    public RectTransform skipTurnButtonProgress;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[LT] Skip Turn Button"), ReadOnly]
    public float skipTurnButtonProgressCounter = 0;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[LT] Skip Turn Button")]
    public float skipTurnButtonProgressMax = 100;

    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] Interaction Button")]
    public Button interactionButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] Interaction Button")]
    public RectTransform interactionButtonProgress;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] Interaction Button"), ReadOnly]
    public float interactionButtonProgressCounter = 0;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] Interaction Button")]
    public float interactionButtonProgressMax = 100;

    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] World Interaction Button")]
    public Button wInteractionButton;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] World Interaction Button")]
    public RectTransform wInteractionButtonProgress;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] World Interaction Button")]
    public Text wInteractionButtonLabel;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] World Interaction Button"), ReadOnly]
    public float wInteractionButtonProgressCounter = 0;
    [FoldoutGroup("Button Configuration"), BoxGroup("Button Configuration/[RT] World Interaction Button")]
    public float wInteractionButtonProgressMax = 100;

    public List<PlayerActionType> instantActions = new List<PlayerActionType>();

    public GameObject enemyTurnFadeImage;


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

        this.walkButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Walk));
        this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
        this.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
        this.torchButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Torch));
        this.skipTurnButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.SkipTurn));
        this.interactionButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Interaction));
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
                    button: this.wInteractionButton,
                    progressTransform: this.wInteractionButtonProgress,
                    progressCounter: ref this.wInteractionButtonProgressCounter,
                    progressCounterMax: this.wInteractionButtonProgressMax,
                    progressBarWidth: this.wInteractionButton.GetComponent<RectTransform>().rect.width
                );
            }
        }
    }

    public void InputListener()
    {
        this.skipTurnButton.gameObject.SetActive(!this.arePlayerControlsLocked);

        if (this.arePlayerControlsLocked)
        {
            this.interactionButton.gameObject.SetActive(false);
        }
        else
        {
            if (Input.GetButtonDown(this.isXboxJoystick ? "Button A" : "Button B"))
            {
                this.torchButton.onClick.Invoke();
            }

            if (Input.GetButtonDown(this.isXboxJoystick ? "Button B" : "Button X"))
            {
                this.walkButton.onClick.Invoke();
            }

            if (Input.GetButtonDown(this.isXboxJoystick ? "Button X" : "Button A"))
            {
                this.flashlightButton.onClick.Invoke();
            }

            if (Input.GetButtonDown("Button Y"))
            {
                this.granadeButton.onClick.Invoke();
            }

            if (this.currentInteractable != null)
            {
                this.HoldButton(
                    buttonName: "Right Trigger",
                    button: this.interactionButton,
                    progressTransform: this.interactionButtonProgress,
                    progressCounter: ref this.interactionButtonProgressCounter,
                    progressCounterMax: this.interactionButtonProgressMax,
                    // TODO: Remove this comment and/or optimize (replaced hardcoded 200)
                    progressBarWidth: this.interactionButton.GetComponent<RectTransform>().rect.width
                );
            }

            this.HoldButton(
                buttonName: "Left Trigger",
                button: this.skipTurnButton,
                progressTransform: this.skipTurnButtonProgress,
                progressCounter: ref this.skipTurnButtonProgressCounter,
                progressCounterMax: this.skipTurnButtonProgressMax,
                // TODO: Remove this comment and/or optimize (replaced hardcoded 300)
                progressBarWidth: this.skipTurnButton.GetComponent<RectTransform>().rect.width
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
        this.walkButtonProc.gameObject.SetActive(isProcActive);
        
        this.flashlightButton.interactable = !isProcActive;
        this.torchButton.interactable = !isProcActive;

        if (isProcActive)
        {
            this.flashlightButton.onClick.RemoveAllListeners();
            this.torchButton.onClick.RemoveAllListeners();
        }
        else
        {
            this.flashlightButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Flashlight));
            this.torchButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Torch));
        }

        if (isProcActive)
        {
            this.granadeButton.interactable = false;
            this.granadeButton.onClick.RemoveAllListeners();
        }
        else if (this.globalCtrl.globalState.GetIntegerParameterFromState("turnsTillGranadeChargeLeft") < 1)
        {
            this.granadeButton.interactable = true;
            this.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));
        }
    }


    public void GranadeChargeEffect(int turnsTillChargeLeft)
    {
        if (turnsTillChargeLeft > 0)
        {
            this.granadeButton.interactable = false;
            this.granadeButton.onClick.RemoveAllListeners();

            this.granadeButtonCooldownLabel.text = turnsTillChargeLeft.ToString();
            this.granadeButtonCooldownLabel.gameObject.SetActive(true);
        }
        else
        {
            this.granadeButton.interactable = true;
            this.granadeButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Granade));

            this.granadeButtonCooldownLabel.gameObject.SetActive(false);
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
                    this.torchButton.image.sprite = this.torchButtonActive;
                    this.torchButton.onClick.RemoveAllListeners();
                    this.torchButton.onClick.AddListener(this.ConfirmAction);

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

    public void SelectInteractable(Interactable interactable, bool isDungeonScene, string buttonLabel)
    {
        this.currentInteractable = interactable;

        if (isDungeonScene)
        {
            this.interactionButton.gameObject.SetActive(true);
        }
        else
        {
            this.wInteractionButtonLabel.text = string.Format("[RT] {0}", (
                string.IsNullOrWhiteSpace(buttonLabel) ? "Use" : buttonLabel
            ));

            this.wInteractionButton.onClick.AddListener(this.currentInteractable.OnClickSync);
            this.wInteractionButton.gameObject.SetActive(true);
        }
    }

    public void DeselectInteractable(bool isDungeonScene)
    {
        this.currentInteractable = null;

        if (isDungeonScene)
        {
            this.interactionButton.gameObject.SetActive(false);
        }
        else
        {
            this.wInteractionButton.onClick.RemoveAllListeners();
            this.wInteractionButton.gameObject.SetActive(false);
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
                this.torchButton.image.sprite = this.torchButtonInactive;
                this.torchButton.onClick.RemoveAllListeners();
                this.torchButton.onClick.AddListener(() => this.SelectActionType(PlayerActionType.Torch));
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
