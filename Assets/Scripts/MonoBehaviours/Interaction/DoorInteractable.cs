using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


public class DoorInteractable : Interactable
{
    public DoorType type;
    public SceneType toScene;
    public Renderer doorRenderer;
    public Animator doorAnimator;
    [InlineEditor]
    public Tile attachedTile;
    public Point exitPoint;
    public PlayerController playerControl;

    public SceneController sceneCtrl;


    public Color defaultColor;
    public Color hoverColor = Color.cyan;

    private readonly int OPEN_DOOR_HASH = Animator.StringToHash("OpenDoor");
    private readonly int ATTEMPT_TAKE_HASH = Animator.StringToHash("AttemptTake");
    private readonly int HIGH_TAKE_HASH = Animator.StringToHash("HighTake");
    private readonly WaitForSeconds DOOR_OPEN_TIMEOUT = new WaitForSeconds(1.5F);


    protected override IEnumerator OnLeftClick()
    {
        switch (this.GetReaction())
        {
            case DoorReaction.GO_TO_DOOR:
                yield return GoToAttachedTile();
                break;
            case DoorReaction.TRY_OPEN_DOOR:
                yield return GoToAttachedTile();
                TryOpenDoor();
                break;
            case DoorReaction.OPEN_DOOR:
                yield return GoToAttachedTile();
                yield return OpenDoor();
                break;
            default:
                Debug.Log("Unexpected DoorReaction");
                break;
        }
    }


    private DoorReaction GetReaction()
    {
        GlobalState globalState = this.sceneCtrl.globalState;

        switch (this.type)
        {
            case DoorType.AptN1_Bedroom_ToLivingRoom:
            case DoorType.AptN1_LivingRoom_ToBedroom:
                if (globalState.playerInventory.Exists(item => item.id == ItemId.AptN1_Bedroom_DoorKey))
                {
                    globalState.sceneStates[this.toScene].position = this.exitPoint;
                    return DoorReaction.OPEN_DOOR;
                }
                else
                {
                    Debug.Log("Can't open door. You need to find 'Door Key'");
                    return DoorReaction.TRY_OPEN_DOOR;
                }
            case DoorType.AptN1_LivingRoom_ToBathroom:
            case DoorType.AptN1_LivingRoom_ToHallway:
            case DoorType.Hallway_AptN0:
            case DoorType.Hallway_AptN1:
            case DoorType.Hallway_AptN2:
            case DoorType.Hallway_AptN3:
            case DoorType.Hallway_AptN4:
            case DoorType.Hallway_AptN5:
            case DoorType.Hallway_Hallway:
            case DoorType.AptN0_LivingRoom_ToHallway:
            case DoorType.AptN1_Bathroom_ToLivingRoom:
            case DoorType.AptN3_Bathroom_ToLivingRoom:
            case DoorType.AptN3_Bedroom_ToLivingRoom:
            case DoorType.AptN3_LivingRoom_ToBedroom:
            case DoorType.AptN3_LivingRoom_ToBathroom:
            case DoorType.AptN3_LivingRoom_ToHallway:
            case DoorType.AptN5_Bathroom_ToLivingRoom:
            case DoorType.AptN5_Bedroom_ToLivingRoom:
            case DoorType.AptN5_LivingRoom_ToBedroom:
            case DoorType.AptN5_LivingRoom_ToBathroom:
            case DoorType.AptN5_LivingRoom_ToHallway:
                if (this.sceneCtrl.sceneState.doors[this.type])
                {
                    globalState.sceneStates[this.toScene].position = this.exitPoint;
                    return DoorReaction.OPEN_DOOR;
                }
                else
                {
                    return DoorReaction.TRY_OPEN_DOOR;
                }
            default:
                Debug.Log("Unexpected DoorType");
                return DoorReaction.GO_TO_DOOR;
        }
    }


    private IEnumerator GoToAttachedTile()
    {
        yield return base.StartCoroutine(this.attachedTile.obj.GetComponent<TileInteractable>().MoveToThisTile());
    }

    private void TryOpenDoor()
    {
        this.playerControl.Interact(ATTEMPT_TAKE_HASH, this.transform.position);
    }

    private IEnumerator OpenDoor()
    {
        this.playerControl.Interact(HIGH_TAKE_HASH, this.transform.position);

        if (this.doorAnimator != null)
        {
            this.doorAnimator.SetTrigger(OPEN_DOOR_HASH);
            yield return DOOR_OPEN_TIMEOUT;
        }

        // Switch scene
        if (this.toScene != SceneType.Undefined)
        {
            Debug.LogFormat("switching to {0} scene", this.toScene);
            SceneManager.FadeAndLoadScene(this.toScene.ToString());
        }
        else
        {
            Debug.Log("sceneName is undefined");
        }
    }

    protected override IEnumerator OnHoverStart()
    {
        yield return null;

        if (this.type == DoorType.Hallway_Hallway)
        {
            this.doorRenderer.material.SetColor("_TintColor", this.hoverColor);
        }
        else
        {
            this.doorRenderer.material.color = this.hoverColor;
        }
    }

    protected override IEnumerator OnHoverEnd()
    {
        yield return new WaitForSeconds(0.1F);

        if (this.type == DoorType.Hallway_Hallway)
        {
            this.doorRenderer.material.SetColor("_TintColor", this.defaultColor);
        }
        else
        {
            this.doorRenderer.material.color = this.defaultColor;
        }
    }
}

public enum DoorType
{
    AptN1_Bedroom_ToLivingRoom,
    AptN1_LivingRoom_ToBedroom,
    AptN1_LivingRoom_ToBathroom,
    AptN1_LivingRoom_ToHallway,
    Hallway_AptN0,
    Hallway_AptN1,
    Hallway_AptN2,
    Hallway_AptN3,
    Hallway_AptN4,
    Hallway_AptN5,
    Hallway_Hallway,
    AptN0_LivingRoom_ToHallway,
    AptN1_Bathroom_ToLivingRoom,
    AptN3_Bathroom_ToLivingRoom,
    AptN3_Bedroom_ToLivingRoom,
    AptN3_LivingRoom_ToBedroom,
    AptN3_LivingRoom_ToBathroom,
    AptN3_LivingRoom_ToHallway,
    AptN5_Bathroom_ToLivingRoom,
    AptN5_Bedroom_ToLivingRoom,
    AptN5_LivingRoom_ToBedroom,
    AptN5_LivingRoom_ToBathroom,
    AptN5_LivingRoom_ToHallway,
}

public enum DoorReaction
{
    GO_TO_DOOR,
    TRY_OPEN_DOOR,
    OPEN_DOOR,
}
