using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class TileData : IDataInteractable
{
    [FoldoutGroup("Default Parameters")]
    public GameObject gameObject { get { return this.tile ? this.tile.obj : null; } }

    [FoldoutGroup("Default Parameters")]
    public Renderer renderer { get { return this.tile ? this.tile.obj.GetComponent<Renderer>() : null; } }


    [FoldoutGroup("Default Parameters")]
    public Animator _animator;
    public Animator animator { get { return this._animator; } }


    [FoldoutGroup("Default Parameters")]
    public Color _defaultColor;
    public Color defaultColor
    {
        get { return this._defaultColor; }
        set { this._defaultColor = value; }
    }


    [FoldoutGroup("Default Parameters")]
    public List<Tile> neighbourTiles { get { return this.tile ? this.tile.allNeighbours.ToList() : null; } }

    [BoxGroup("Tile Parameters")]
    [InlineEditor]
    public Tile tile;

    [BoxGroup("Tile Parameters")]
    public Color disabledColor = new Color(1F, 1F, 1F, 0F);


    public void RefreshTileState(bool isVisible, bool isBlocked, bool inEditor)
    {                
        Material material = inEditor ? this.renderer.sharedMaterial : this.renderer.material;

        this.tile.isVisible = isVisible;
        this.tile.isBlocked = isBlocked;

        Color nextColor = (!isBlocked && isVisible) ? this.defaultColor : this.disabledColor;
        material.SetColor(Shader.PropertyToID("_Color"), nextColor);
    }


#if UNITY_EDITOR

    [ButtonGroup("Editor Controls/Tile Controls")]
    [Button("Active", ButtonSizes.Medium)]
    public void MakeActiveInEditor()
    {
        this.RefreshTileState(true, false, true);
    }

    [ButtonGroup("Editor Controls/Tile Controls")]
    [Button("Hidden", ButtonSizes.Medium)]
    public void MakeHiddenInEditor()
    {
        this.RefreshTileState(false, false, true);
    }

    [ButtonGroup("Editor Controls/Tile Controls")]
    [Button("Disabled", ButtonSizes.Medium)]
    public void MakeDisabledInEditor()
    {
        this.RefreshTileState(false, true, true);
    }

    [BoxGroup("Editor Controls")]
    public Interactable interactable;

    [BoxGroup("Editor Controls")]
    [Button(ButtonSizes.Medium)]
    public void CopyTilesToInteractableData()
    {
        LightSourceData lsd = this.interactable.data as LightSourceData;
        lsd.highlightedTiles.Add(this.tile);
    }

#endif
}

