using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalStateController : MonoBehaviour
{
    public List<Item> playerInventory = new List<Item>();

    public Dictionary<DoorType, bool> doors = new Dictionary<DoorType, bool>()
    {
        { DoorType.AptN1_Bedroom_ToLivingRoom, false },
        { DoorType.AptN1_LivingRoom_ToBathroom, false },
        { DoorType.AptN1_LivingRoom_ToBedroom, false },
        { DoorType.AptN1_LivingRoom_ToHallway, false },
    };

    public Dictionary<SceneType, Point> positionInScene = new Dictionary<SceneType, Point>()
    {
        { SceneType.AptN1_Bedroom, new Point(1, 4) },
        { SceneType.AptN1_LivingRoom, new Point(6, 5) },
    };

    public List<Item> bedroomTable = new List<Item>();

    public Dictionary<ContainerType, List<Item>> containers = new Dictionary<ContainerType, List<Item>>();


    private void Awake()
    {
        this.containers = new Dictionary<ContainerType, List<Item>>()
        {
            { ContainerType.AptN1_Bedroom_Table, this.bedroomTable },
            { ContainerType.AptN1_Bedroom_Bed, new List<Item>() },
        };
    }
}

[Flags]
public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
}
