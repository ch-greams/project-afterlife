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
    public Color defaultColor { get { return this._defaultColor; } }


    [FoldoutGroup("Default Parameters")]
    public List<Tile> neighbourTiles { get { return this.tile ? this.tile.allNeighbours.ToList() : null; } }


    [InlineEditor]
    public Tile tile;


#if UNITY_EDITOR

    [ButtonGroup("Tile Controls")]
    [Button(ButtonSizes.Medium)]
    public void MakePossible()
    {
        this.tile.passable = true;
    }

    [ButtonGroup("Tile Controls")]
    [Button(ButtonSizes.Medium)]
    public void MakeImpossible()
    {
        this.tile.passable = false;
    }

#endif
}

