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

    [InlineEditor]
    public Tile tile;

    public Color disabledColor = new Color(1F, 1F, 1F, 0F);


    public void RefreshTileState(TileState tileState, bool inEditor)
    {
        this.tile.state = tileState;
        Material material = inEditor ? this.renderer.sharedMaterial : this.renderer.material;

        switch (this.tile.state)
        {
            case TileState.Active:
                material.SetColor(Shader.PropertyToID("_Color"), this.defaultColor);
                break;
            case TileState.Hidden:
            case TileState.Disabled:
            default:
                material.SetColor(Shader.PropertyToID("_Color"), this.disabledColor);
                break;
        }
    }


#if UNITY_EDITOR

    [ButtonGroup("Tile Controls")]
    [Button("Active", ButtonSizes.Medium)]
    public void MakeActiveInEditor()
    {
        this.RefreshTileState(TileState.Active, true);
    }

    [ButtonGroup("Tile Controls")]
    [Button("Hidden", ButtonSizes.Medium)]
    public void MakeHiddenInEditor()
    {
        this.RefreshTileState(TileState.Hidden, true);
    }

    [ButtonGroup("Tile Controls")]
    [Button("Disabled", ButtonSizes.Medium)]
    public void MakeDisabledInEditor()
    {
        this.RefreshTileState(TileState.Disabled, true);
    }

#endif
}

