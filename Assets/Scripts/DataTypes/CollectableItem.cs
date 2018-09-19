using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem
{
    public CollectableItemType type;
    public string name;
    public GameObject obj;


    public CollectableItem(string name, GameObject obj)
    {
        this.type = CollectableItemType.Healthpack;

        this.name = name;

        this.obj = obj;
        this.obj.name = name;
    }


    public void Destroy()
    {
        GameObject.DestroyImmediate(this.obj);
    }
}

public enum CollectableItemType
{
    Healthpack,
    // TempVisibilityBuff,
}
