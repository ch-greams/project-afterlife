using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable, CreateAssetMenu]
public class Objective : SerializedScriptableObject
{
    public ObjectiveId id;
    public string title;
    public bool completed;
    public List<Objective> nextObjectives = new List<Objective>();

    [GUIColor(0.7F, 0.7F, 0.7F, 1F)]
    [DictionaryDrawerSettings(KeyLabel = "Task", DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, Task> tasks = new Dictionary<string, Task>();


    public void Complete(string taskId, string subTaskId)
    {
        this.tasks[taskId].Complete(subTaskId);

        if (this.tasks.All(kvp => kvp.Value.completed || kvp.Value.optional))
        {
            this.completed = true;
            Debug.LogFormat("objective '{0} is complete", this.title);
        }
    }
}

public enum ObjectiveId
{
    Undefined,
    Intro_WakeUp,
    Intro_GetReady,
    Intro_GoToWork,
}
