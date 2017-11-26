using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class GlobalStateController : SerializedMonoBehaviour
{
    [InlineEditor(Expanded = true)]
    public GlobalState globalState;

    public List<Image> playerInventorySlots = new List<Image>();

    public void Awake()
    {
        this.globalState = SerializedScriptableObject.Instantiate(this.globalState);
    }
}

[Flags]
public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
}
