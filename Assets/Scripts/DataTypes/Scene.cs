using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Scene
{
    [Required]
    public SceneController sceneCtrl;

    public SceneType type;


    public Scene() { }

    public void UpdateStartPoint(SceneType type, Point point)
    {
        this.sceneCtrl.globalState.sceneStates[type].position = point;
    }
}

public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
    Hallway,
    AptN3_LivingRoom,
    AptN0_LivingRoom,
    AptN1_Bathroom,
    AptN3_Bathroom,
    AptN3_Bedroom,
    AptN5_Bathroom,
    AptN5_Bedroom,
    AptN5_LivingRoom,
}
