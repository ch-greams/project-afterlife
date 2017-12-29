using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class DoorData : IDataInteractable
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
    public Color defaultColor { get { return this._defaultColor; } }

    [FoldoutGroup("Default Parameters")]
    public List<Tile> _neighbourTiles = new List<Tile>();
    public List<Tile> neighbourTiles { get { return this._neighbourTiles; } }


    [BoxGroup("Door Parameters")]
    public SceneType scene;
    [BoxGroup("Door Parameters")]
    public Point exitPosition;
}
