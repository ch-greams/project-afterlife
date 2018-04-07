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
