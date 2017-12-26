﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class SceneState : SerializedScriptableObject
{
    public Point position;
    
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<DoorType, bool> doors = new Dictionary<DoorType, bool>();
}
