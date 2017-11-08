using System.Collections;
using UnityEngine;


public class DoorInteractable : Interactable
{
    public Renderer doorRenderer;
    public Animator doorAnimator;
    public CellInteractable attachedCell;
    public PlayerControl playerControl;
    public string sceneName;
    public SceneController sceneController;

    private Color defaultColor;

	private readonly Color HOVER_COLOR = Color.cyan;
    private readonly int OPEN_DOOR_HASH = Animator.StringToHash("OpenDoor");
    private readonly int HIGH_TAKE_HASH = Animator.StringToHash("HighTake");
    private readonly WaitForSeconds DOOR_OPEN_TIMEOUT = new WaitForSeconds(1.5F);


    private void Start()
    {
        this.defaultColor = this.doorRenderer.material.color;
        this.sceneController = FindObjectOfType<SceneController>();
    }

    protected override IEnumerator OnLeftClick()
    {
        // Go To The Door
        yield return base.StartCoroutine(this.attachedCell.MoveToThisCell());

        // Open Door
        this.playerControl.Interact(HIGH_TAKE_HASH, this.transform.position);
        this.doorAnimator.SetTrigger(OPEN_DOOR_HASH);
        yield return DOOR_OPEN_TIMEOUT;

        // Switch scene
        if (!string.IsNullOrEmpty(this.sceneName))
        {
            Debug.Log("switching scene");
            this.sceneController.FadeAndLoadScene(this.sceneName);
        }
        else
        {
            Debug.Log("sceneName is undefined");
        }
    }

    protected override IEnumerator OnHoverStart()
    {
        yield return null;
        this.doorRenderer.material.color = this.HOVER_COLOR;
    }

    protected override IEnumerator OnHoverEnd()
    {
        yield return new WaitForSeconds(0.1F);
        this.doorRenderer.material.color = this.defaultColor;
    }
}
