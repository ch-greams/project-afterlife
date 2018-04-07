using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class Task
{
    public bool completed;
    public bool optional;

    [GUIColor(1F, 1F, 1F, 1F)]
    [ValidateInput("ValidateUniqueIds", "All IDs should be unique.")]
    [ListDrawerSettings(ListElementLabelName = "id", Expanded = false)]
    public List<SubTask> subTasks = new List<SubTask>();


    public void Complete(string subTaskId)
    {
        string currentTaskTitle = this.GetCurrentVisibleSubTask().title;
        this.subTasks.Find(st => st.id == subTaskId).Complete();

        if (this.subTasks.Last().id == subTaskId)
        {
            this.completed = true;
            Debug.LogFormat("task '{0}' is complete", currentTaskTitle);
        }
    }

    public SubTask GetCurrentVisibleSubTask()
    {
        return this.subTasks.Exists(st => !st.hidden && !st.completed)
            ? this.subTasks.Find(st => !st.hidden && !st.completed)
            : this.subTasks.FindLast(st => !st.hidden);
    }

    public SubTask GetCurrentSubTask()
    {
        return this.subTasks.Find(st => !st.optional && !st.completed);
    }

    private bool ValidateUniqueIds(List<SubTask> subTasks)
    {
        HashSet<string> ids = new HashSet<string>();
        return (subTasks == null) || subTasks.All(subTask => ids.Add(subTask.id));
    }


    public void LoadFromSerializable(TaskSerializable serializedTask)
    {
        this.completed = serializedTask.completed;
        this.optional = serializedTask.optional;

        foreach (SubTask subTask in this.subTasks)
        {
            subTask.LoadFromSerializable(serializedTask.subTasks[subTask.id]);
        }
    }
}

[Serializable]
public class TaskSerializable
{
    public bool completed;
    public bool optional;
    public Dictionary<string, SubTaskSerializable> subTasks = new Dictionary<string, SubTaskSerializable>();


    public TaskSerializable(Task task)
    {
        this.completed = task.completed;
        this.optional = task.optional;

        this.subTasks = task.subTasks.ToDictionary(
            subTask => subTask.id,
            subTask => new SubTaskSerializable(subTask)
        );
    }
}
