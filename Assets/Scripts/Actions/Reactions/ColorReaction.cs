﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class ColorReaction : IReaction
{
    public ColorReactionType type = ColorReactionType.DEFAULT_RENDERER;
    [ShowIf("type", ColorReactionType.CUSTOM_RENDERERS)]
    public List<Renderer> customRenderers = new List<Renderer>();
    public string propertyName = "_Color";
    [ColorPalette]
    public Color color;

    private Renderer defaultRenderer;
    private int propertyId;


    public void Init(Interactable interactable)
    {
        this.propertyId = Shader.PropertyToID(this.propertyName);
        this.defaultRenderer = interactable.data.renderer;
    }
    public IEnumerator React()
    {
        switch (this.type)
        {
            case ColorReactionType.DEFAULT_RENDERER:
                this.defaultRenderer.material.SetColor(this.propertyId, this.color);
                break;
            case ColorReactionType.CUSTOM_RENDERERS:
                this.customRenderers.ForEach(renderer => renderer.material.SetColor(this.propertyId, this.color));
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum ColorReactionType
{
    DEFAULT_RENDERER,
    CUSTOM_RENDERERS,
}
