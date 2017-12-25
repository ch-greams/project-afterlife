using System;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class SubTask
{
    [HideIf("hidden")]
    public string title;
    public bool completed;
    public bool optional;
    public bool hidden;
    public bool finalSubTask;
    [HideIf("finalSubTask")]
    public string nextSubTaskId;


    public void Complete()
    {
        this.completed = true;
        Debug.LogFormat("subTask '{0}' is complete", this.title);
    }
}
