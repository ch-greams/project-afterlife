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
        string currentTaskTitle = this.GetCurrentTaskTitle();
        this.subTasks.Find(st => st.id == subTaskId).Complete();

        if (this.subTasks.Last().id == subTaskId)
        {
            this.completed = true;
            Debug.LogFormat("task '{0}' is complete", currentTaskTitle);
        }
    }

    public string GetCurrentTaskTitle()
    {
        return this.subTasks.Exists(st => !st.hidden && !st.completed)
            ? this.subTasks.Find(st => !st.hidden && !st.completed).title
            : this.subTasks.FindLast(st => !st.hidden).title;
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
}
