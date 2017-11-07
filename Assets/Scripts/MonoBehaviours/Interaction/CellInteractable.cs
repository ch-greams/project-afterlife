using System.Collections;
using UnityEngine;


public class CellInteractable : Interactable
{
	public PlayerControl playerControl;
	private AnimateTiledTexture textureAnimator;


	private void Start()
	{
		this.textureAnimator = this.GetComponent<AnimateTiledTexture>();
	}

    protected override IEnumerator OnLeftClick()
	{
		yield return this.MoveToThisCell();
	}

    public IEnumerator MoveToThisCell()
    {
        this.textureAnimator.Play();
        yield return this.playerControl.MoveToAsync(base.transform.position);
    }
}
