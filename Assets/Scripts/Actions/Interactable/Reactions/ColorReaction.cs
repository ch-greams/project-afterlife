using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class ColorReaction : IInteractableReaction
{
    public ColorReactionType type = ColorReactionType.DEFAULT_RENDERER_HIGHLIGHT;

    [ShowIf("type", ColorReactionType.CUSTOM_RENDERERS)]
    public List<Renderer> customRenderers = new List<Renderer>();

    [HideIf("type", ColorReactionType.DEFAULT_RENDERER_RESET)]
    public string propertyName = "_Color";

    [HideIf("type", ColorReactionType.DEFAULT_RENDERER_RESET)]
    [ColorPalette]
    public Color color;

    private IDataInteractable interactableData;

    private Renderer defaultRenderer;
    private int propertyId;


    public void Init(Interactable interactable)
    {
        this.propertyId = Shader.PropertyToID(this.propertyName);
        this.interactableData = interactable.data;
        this.defaultRenderer = this.interactableData.renderer;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case ColorReactionType.DEFAULT_RENDERER_HIGHLIGHT:
                this.defaultRenderer.material.SetColor(this.propertyId, this.color);
                break;
            case ColorReactionType.CUSTOM_RENDERERS:
                this.customRenderers.ForEach(renderer => renderer.material.SetColor(this.propertyId, this.color));
                break;
            case ColorReactionType.DEFAULT_RENDERER_RESET:
                this.defaultRenderer.material.SetColor(this.propertyId, this.interactableData.defaultColor);
                break;
            case ColorReactionType.DEFAULT_RENDERER_UPDATE_COLOR:
                this.interactableData.defaultColor = this.color;
                this.defaultRenderer.material.SetColor(this.propertyId, this.color);
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum ColorReactionType
{
    DEFAULT_RENDERER_HIGHLIGHT,
    CUSTOM_RENDERERS,
    DEFAULT_RENDERER_RESET,
    DEFAULT_RENDERER_UPDATE_COLOR,
}
