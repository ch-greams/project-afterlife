using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class Task
{
    public string defaultTitle;
    public bool completed;
    public bool optional;
    public string currentSubTaskId;

    [GUIColor(1F, 1F, 1F, 1F)]
    [DictionaryDrawerSettings(KeyLabel = "Task Step", DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, SubTask> subTasks = new Dictionary<string, SubTask>();


    public void Complete(string subTaskId)
    {
        SubTask subTask = this.subTasks[subTaskId];
        subTask.Complete();

        if (subTask.finalSubTask)
        {
            this.completed = true;
            Debug.LogFormat("task '{0}' is complete", this.defaultTitle);
        }
    }
}
