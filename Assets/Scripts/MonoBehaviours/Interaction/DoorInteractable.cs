using System.Collections;
using UnityEngine;


public class DoorInteractable : Interactable
{
    public Renderer doorRenderer;
    public Animator doorAnimator;
    public CellInteractable attachedCell;
    public string sceneName;
    public SceneController sceneController;

    private Color defaultColor;

	private readonly Color hoverColor = Color.cyan;
    private readonly int OPEN_DOOR_HASH = Animator.StringToHash("OpenDoor");


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
        this.doorAnimator.SetTrigger(OPEN_DOOR_HASH);

        // Switch scene
        if (!string.IsNullOrEmpty(this.sceneName))
        {
            Debug.Log("switching scene");
            base.StartCoroutine(this.sceneController.FadeAndSwitchScenes(this.sceneName));
        }
        else
        {
            Debug.Log("sceneName is undefined");
        }
    }

    protected override IEnumerator OnHoverStart()
    {
        yield return null;
        this.doorRenderer.material.color = this.hoverColor;
    }

    protected override IEnumerator OnHoverEnd()
    {
        yield return new WaitForSeconds(0.1F);
        this.doorRenderer.material.color = this.defaultColor;
    }
}
