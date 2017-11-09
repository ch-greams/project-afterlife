using System.Collections;
using UnityEngine;


public class TileInteractable : Interactable
{
	public Tile tile;
	public PlayerControl playerControl;
	private AnimateTiledTexture textureAnimator;


	private void Start()
	{
		this.textureAnimator = this.GetComponent<AnimateTiledTexture>();
	}

    protected override IEnumerator OnLeftClick()
	{
		yield return this.MoveToThisTile();
	}

    public IEnumerator MoveToThisTile()
    {
        this.textureAnimator.Play();
        yield return this.playerControl.MoveToAsync(base.transform.position);
    }
}
