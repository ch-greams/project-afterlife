using System;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class SubTask
{
    [PropertyOrder(1)]
    public string title;
    [ShowInInspector, PropertyOrder(0)]
    public string id { get { return (this.title != null ? this.title.Replace(" ", "_") : ""); } }
    [PropertyOrder(2)]
    public bool completed;
    [PropertyOrder(3)]
    public bool optional;
    [PropertyOrder(4)]
    public bool hidden;


    public void Complete()
    {
        this.completed = true;
        Debug.LogFormat("subTask '{0}' is complete", this.id);
    }


    public void LoadFromSerializable(SubTaskSerializable serializedSubTask)
    {
        this.title = serializedSubTask.title;

        this.completed = serializedSubTask.completed;
        this.optional = serializedSubTask.optional;
    }
}

[Serializable]
public class SubTaskSerializable
{
    public string id;
    public string title;
    public bool completed;
    public bool optional;


    public SubTaskSerializable(SubTask subTask)
    {
        this.id = subTask.id;
        this.title = subTask.title;

        this.completed = subTask.completed;
        this.optional = subTask.optional;
    }
}
