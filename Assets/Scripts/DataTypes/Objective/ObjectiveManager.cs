using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class ObjectiveManager
{
    public Text objectiveTitle;
    public GameObject taskList;

    [FoldoutGroup("Task")]
    public GameObject taskPrefab;

    [BoxGroup("Task/Active Task")]
    public Font activeTaskFont;
    [BoxGroup("Task/Active Task")]
    public Color activeTaskColor;

    [BoxGroup("Task/Completed Task")]
    public Font completedTaskFont;
    [BoxGroup("Task/Completed Task")]
    public Color completedTaskColor;

    public Dictionary<string, GameObject> tasks = new Dictionary<string, GameObject>();

    private GlobalState globalState;
    private DialogueManager dialogueManager;
    

    public void Init(GlobalController globalCtrl)
    {
        this.globalState = globalCtrl.globalState;
        this.dialogueManager = globalCtrl.dialogueManager;

        this.UpdateObjective();
    }

    public void CompleteSubTask(ObjectiveId objectiveId, string taskId, string subTaskId)
    {
        Objective objective = this.globalState.objectives[objectiveId];
        objective.Complete(taskId, subTaskId);

        if (objective.completed)
        {
            this.globalState.currentObjective = objective.nextObjectives[0].id;
        }

        this.UpdateObjective();
    }

    private void UpdateObjective()
    {
        Objective objective = this.globalState.objectives[this.globalState.currentObjective];

        this.objectiveTitle.text = objective.title;

        foreach (Transform task in this.taskList.transform)
        {
            GameObject.Destroy(task.gameObject);
        }

        this.tasks = new Dictionary<string, GameObject>();

        foreach (KeyValuePair<string, Task> kvp in objective.tasks)
        {
            this.tasks.Add(kvp.Key, this.CreateTask(kvp.Value));
        }
    }

    private GameObject CreateTask(Task taskData)
    {
        SubTask currentSubTask = taskData.GetCurrentVisibleSubTask();

        GameObject task = GameObject.Instantiate(this.taskPrefab);
        Text taskText = task.GetComponentInChildren<Text>();
        taskText.text = currentSubTask.title;
        Button taskButton = task.GetComponent<Button>();
        taskButton.onClick.AddListener(() => this.dialogueManager.StartDialogue(currentSubTask.comment));
        taskButton.interactable = !taskData.completed;

        if (taskData.completed)
        {
            taskText.font = this.completedTaskFont;
            taskText.color = this.completedTaskColor;
        }
        else
        {
            taskText.font = this.activeTaskFont;
            taskText.color = this.activeTaskColor;
        }

        task.transform.SetParent(this.taskList.transform, false);

        return task;
    }


    [BoxGroup("Task/Active Task")]
    [Button]
    public void AddActiveTask()
    {
        Debug.Log("AddActiveTask");

        GameObject task = GameObject.Instantiate(this.taskPrefab);
        Text taskText = task.GetComponentInChildren<Text>();
        taskText.font = this.activeTaskFont;
        taskText.color = this.activeTaskColor;
        task.transform.SetParent(this.taskList.transform, false);
    }

    [BoxGroup("Task/Completed Task")]
    [Button]
    public void AddCompletedTask()
    {
        Debug.Log("AddCompletedTask");

        GameObject task = GameObject.Instantiate(this.taskPrefab);
        Text taskText = task.GetComponentInChildren<Text>();
        taskText.font = this.completedTaskFont;
        taskText.color = this.completedTaskColor;
        task.transform.SetParent(this.taskList.transform, false);
    }
}
