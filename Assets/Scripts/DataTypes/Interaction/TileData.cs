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


    public void RefreshTileMaterial(Tile tile, bool inEditor)
    {                
        Material material = inEditor ? this.renderer.sharedMaterial : this.renderer.material;
        int shaderPropID = Shader.PropertyToID("_Color");
        
        SceneController sceneCtrl = this.tile.sceneCtrl;

        if (tile.isSelected)
        {
            // TILE_COLOR_SELECTED
            material.SetColor(shaderPropID, sceneCtrl.selectedTileColor);
        }
        else if (!tile.isBlocked && tile.isActive)
        {
            // TILE_COLOR_ACTIVE
            material.SetColor(shaderPropID, sceneCtrl.activeTileColor);
        }
        else if (!tile.isBlocked && tile.isVisible)
        {
            // TILE_COLOR_VISIBLE
            material.SetColor(shaderPropID, sceneCtrl.visibleTileColor);
        }
        else
        {
            // TILE_COLOR_DISABLED as default
            material.SetColor(shaderPropID, sceneCtrl.disabledTileColor);
        }
    }


#if UNITY_EDITOR

    [ButtonGroup("Editor Controls")]
    [Button("Active", ButtonSizes.Medium)]
    public void MakeActiveInEditor()
    {
        this.tile.RefreshTileState(true, false, true);
    }

    [ButtonGroup("Editor Controls")]
    [Button("Hidden", ButtonSizes.Medium)]
    public void MakeHiddenInEditor()
    {
        this.tile.RefreshTileState(false, false, true);
    }

    [ButtonGroup("Editor Controls")]
    [Button("Disabled", ButtonSizes.Medium)]
    public void MakeDisabledInEditor()
    {
        this.tile.RefreshTileState(false, true, true);
    }

#endif
}

