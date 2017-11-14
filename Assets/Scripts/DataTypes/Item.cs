using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Item : ScriptableObject
{
    public ItemId id;
    public string label;
}

public enum ItemId
{
    AptN1_Bedroom_DoorKey,
}