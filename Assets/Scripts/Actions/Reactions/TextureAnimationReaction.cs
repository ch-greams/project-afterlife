using System.Collections;
using UnityEngine;


public class TextureAnimationReaction : IReaction
{
    public TextureAnimationReactionType type = TextureAnimationReactionType.ASYNC_ANIMATION;
    private Renderer renderer;
    public int framesPerSecond = 50;

    public int rows = 4;
    public int columns = 4;

    public string propertyName = "_MainTex";

    private int propertyId;
    private Vector2 textureSize;
    private WaitForSeconds timeoutBetweenFrames;

    private Interactable interactable;


    public void Init(Interactable interactable)
    {
        this.propertyId = Shader.PropertyToID(this.propertyName);
        this.textureSize = new Vector2((1F / this.columns), (1F / this.rows));
        this.timeoutBetweenFrames = new WaitForSeconds(1F / this.framesPerSecond);

        this.interactable = interactable;
        this.renderer = (interactable as TileInteractable).tile.obj.GetComponent<Renderer>();

        // Create new material instance & destroy and/or reassign new material instance
        this.renderer.sharedMaterial = new Material(this.renderer.sharedMaterial);
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case TextureAnimationReactionType.ASYNC_ANIMATION:
                this.interactable.StartCoroutine(this.Animate());
                break;
            case TextureAnimationReactionType.SYNC_ANIMATION:
                yield return this.Animate();
                break;
            default:
                yield return null;
                break;
        }
    }

    private IEnumerator Animate()
    {
        for (int y = 0; y < this.rows; y++)
        {
            for (int x = 0; x < this.columns; x++)
            {
                float xi = this.textureSize.x * x;
                float yi = 1F - (this.textureSize.y * (y + 1));

                this.renderer.sharedMaterial.SetTextureOffset(this.propertyId, new Vector2(xi, yi));

                yield return timeoutBetweenFrames;
            }
        }
    }
}

public enum TextureAnimationReactionType
{
    ASYNC_ANIMATION,
    SYNC_ANIMATION,
}
