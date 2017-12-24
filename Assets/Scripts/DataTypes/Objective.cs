using System;
using System.Collections.Generic;
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
}

public enum ObjectiveId
{
    Undefined,
    Intro_WakeUp,
    Intro_GetReady,
    Intro_GoToWork,
}


[Serializable]
public class Task
{
    public string defaultTitle;
    public bool completed;
    public bool optional;

    [GUIColor(1F, 1F, 1F, 1F)]
    [DictionaryDrawerSettings(KeyLabel = "Task Step", DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, SubTask> subTasks = new Dictionary<string, SubTask>();
}


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
}
