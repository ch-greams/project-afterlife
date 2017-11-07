using System;
using UnityEngine;


[Serializable]
public class Cell 
{
    public Point point;
    public GameObject obj;

    public Cell(Point point, GameObject obj)
    {
        this.point = point;
        this.obj = obj;
    }
}