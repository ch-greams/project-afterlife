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
    public Color defaultColor
    {
        get { return this._defaultColor; }
        set { this._defaultColor = value; }
    }

    [FoldoutGroup("Default Parameters")]
    public List<Point> _reachablePoints = new List<Point>();
    public List<Point> reachablePoints { get { return this._reachablePoints; } }


    [BoxGroup("Door Parameters"), ValueDropdown("sceneNames")]
    public string sceneName;
    
    [BoxGroup("Door Parameters")]
    public Point exitPosition;

    private List<string> sceneNames { get { return GlobalController.sceneNames; } }
}
