using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class Objective : SerializedScriptableObject
{
    [ShowInInspector, PropertyOrder(0)]
    public string id { get { return base.name; } }

    [PropertyOrder(1)]
    public string title;
    
    [PropertyOrder(2)]
    public bool completed;
    
    [PropertyOrder(3)]
    public DialogueCollection comment;
    
    [PropertyOrder(4)]
    public Objective nextObjective;

    [PropertyOrder(5), GUIColor(0.7F, 0.7F, 0.7F, 1F)]
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


    public void LoadFromSerializable(ObjectiveSerializable serializedObjective)
    {
        this.completed = serializedObjective.completed;

        foreach (KeyValuePair<string, Task> kvp in this.tasks)
        {
            kvp.Value.LoadFromSerializable(serializedObjective.tasks[kvp.Key]);
        }
    }
}

[Serializable]
public class ObjectiveSerializable
{
    public string id;
    public string title;
    public bool completed;

    public Dictionary<string, TaskSerializable> tasks = new Dictionary<string, TaskSerializable>();


    public ObjectiveSerializable(Objective objective)
    {
        // NOTE: Not used for loading
        this.id = objective.id;
        this.title = objective.title;

        this.completed = objective.completed;
        this.tasks = objective.tasks.ToDictionary(kvp => kvp.Key, kvp => new TaskSerializable(kvp.Value));
    }
}
