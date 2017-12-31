using System;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class SubTask
{
    public string id;
    [HideIf("hidden")]
    public string title;
    [HideIf("hidden")]
    public Dialogue comment;
    public bool completed;
    public bool optional;
    public bool hidden;


    public void Complete()
    {
        this.completed = true;
        Debug.LogFormat("subTask '{0}' is complete", this.id);
    }
}
