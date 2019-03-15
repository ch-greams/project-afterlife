using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class ObjectiveCondition : IInteractableCondition
{
    public ObjectiveConditionType type;

    // NOTE: Use only as a reference for constant values in there
    public Objective objective;

    private List<string> taskIds
    {
        get
        {
            return (this.objective != null)
                ? new List<string>(this.objective.tasks.Keys)
                : new List<string>();
        }
    }

    [HideIf("type", ObjectiveConditionType.ObjectiveIsComplete, false)]
    [HideIf("type", ObjectiveConditionType.ObjectiveIsNotComplete, false)]
    [ValueDropdown("taskIds")]
    public string taskId;

    private List<string> subTaskIds
    {
        get
        {
            return ((this.objective != null) && (this.taskId != null) && this.objective.tasks.ContainsKey(this.taskId))
                ? this.objective.tasks[this.taskId].subTasks.Select(st => st.id).ToList()
                : new List<string>();
        }
    }

    [HideIf("type", ObjectiveConditionType.ObjectiveIsComplete, false)]
    [HideIf("type", ObjectiveConditionType.ObjectiveIsNotComplete, false)]
    [HideIf("type", ObjectiveConditionType.TaskIsComplete, false)]
    [HideIf("type", ObjectiveConditionType.TaskIsNotComplete, false)]
    [ValueDropdown("subTaskIds")]
    public string subTaskId;

    private GlobalState globalState;


    public void Init(Interactable interactable)
    {
        this.globalState = interactable.sceneCtrl.globalState;
    }

    public bool IsValid()
    {
        Objective objective = this.globalState.objectives[this.objective.id];

        switch (this.type)
        {
            case ObjectiveConditionType.ObjectiveIsComplete:
                return objective.completed;
            case ObjectiveConditionType.ObjectiveIsNotComplete:
                return !objective.completed;
            case ObjectiveConditionType.TaskIsComplete:
                return objective.tasks[this.taskId].completed;
            case ObjectiveConditionType.TaskIsNotComplete:
                return !objective.tasks[this.taskId].completed;
            case ObjectiveConditionType.SubTaskIsComplete:
                return objective.tasks[this.taskId].subTasks.Find(st => (st.id == this.subTaskId)).completed;
            case ObjectiveConditionType.SubTaskIsNotComplete:
                return !objective.tasks[this.taskId].subTasks.Find(st => (st.id == this.subTaskId)).completed;
            case ObjectiveConditionType.SubTaskCanBeComplete:
                List<SubTask> subTasks = objective.tasks[this.taskId].subTasks;
                int subTaskIndex = subTasks.FindIndex(st => (st.id == this.subTaskId));
                return
                (
                    // should be current objective
                    (this.objective.id == this.globalState.currentObjective) &&
                    // all required subTasks before should be complete
                    subTasks.Take(subTaskIndex).All(st => (st.optional || st.completed)) &&
                    // current subTask and every one after should not be complete
                    subTasks.Skip(subTaskIndex).All(st => !st.completed)
                );
            default:
                return false;
        }
    }
}

public enum ObjectiveConditionType
{
    ObjectiveIsComplete,
    ObjectiveIsNotComplete,
    TaskIsComplete,
    TaskIsNotComplete,
    SubTaskIsComplete,
    SubTaskIsNotComplete,
    SubTaskCanBeComplete,
}
