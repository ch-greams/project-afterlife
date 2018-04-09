using System.Collections;
using UnityEngine;


public class RendererReaction : IReaction
{
    public bool enable;

    private Renderer defaultRenderer;


    public void Init(Interactable interactable)
    {
        this.defaultRenderer = interactable.data.renderer;
    }
    public IEnumerator React()
    {
        this.defaultRenderer.enabled = this.enable;
        yield return null;
    }
}
