using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class LightSourceData : IDataInteractable
{
    [FoldoutGroup("Default Parameters")]
    public GameObject _gameObject;
    public GameObject gameObject { get { return this._gameObject; } }

    [FoldoutGroup("Default Parameters")]
    public Renderer _renderer;
    public Renderer renderer { get { return this._renderer; } }

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
    public List<Tile> _neighbourTiles = new List<Tile>();
    public List<Tile> neighbourTiles { get { return this._neighbourTiles; } }

    [BoxGroup("LightSource Parameters")]
    public string id;

    [BoxGroup("LightSource Parameters")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10)]
    public List<Tile> highlightedTiles = new List<Tile>();

}
