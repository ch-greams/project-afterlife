using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Door
{
    [Required]
    public SceneController sceneCtrl;

    public DoorType type;
    public GameObject obj;
    public Renderer renderer;
    public Animator animator;
    
    public SceneType toScene;
    public Point exitPoint;


    public Door() { }
}

public enum DoorType
{
    AptN1_Bedroom_ToLivingRoom,
    AptN1_LivingRoom_ToBedroom,
    AptN1_LivingRoom_ToBathroom,
    AptN1_LivingRoom_ToHallway,
    Hallway_AptN0,
    Hallway_AptN1,
    Hallway_AptN2,
    Hallway_AptN3,
    Hallway_AptN4,
    Hallway_AptN5,
    Hallway_Hallway,
    AptN0_LivingRoom_ToHallway,
    AptN1_Bathroom_ToLivingRoom,
    AptN3_Bathroom_ToLivingRoom,
    AptN3_Bedroom_ToLivingRoom,
    AptN3_LivingRoom_ToBedroom,
    AptN3_LivingRoom_ToBathroom,
    AptN3_LivingRoom_ToHallway,
    AptN5_Bathroom_ToLivingRoom,
    AptN5_Bedroom_ToLivingRoom,
    AptN5_LivingRoom_ToBedroom,
    AptN5_LivingRoom_ToBathroom,
    AptN5_LivingRoom_ToHallway,
}
