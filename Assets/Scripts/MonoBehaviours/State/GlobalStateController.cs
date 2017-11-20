using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class GlobalStateController : SerializedMonoBehaviour
{
    public List<Item> playerInventory = new List<Item>();

    public List<Image> playerInventorySlots = new List<Image>();


    public Dictionary<DoorType, bool> doors = new Dictionary<DoorType, bool>();

    public Dictionary<SceneType, Point> positionInScene = new Dictionary<SceneType, Point>();

    public Dictionary<ContainerType, List<Item>> containers = new Dictionary<ContainerType, List<Item>>();


}

[Flags]
public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
}
