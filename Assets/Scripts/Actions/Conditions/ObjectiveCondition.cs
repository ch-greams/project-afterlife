using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class ObjectiveCondition : ICondition
{
    public ObjectiveConditionType type;

    public ObjectiveId objectiveId;

    // NOTE: Editor use only, don't use this variable in Init/IsValid methods
    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_COMPLETE, false)]
    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE, false)]
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

    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_COMPLETE, false)]
    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE, false)]
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

    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_COMPLETE, false)]
    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE, false)]
    [HideIf("type", ObjectiveConditionType.TASK_IS_COMPLETE, false)]
    [HideIf("type", ObjectiveConditionType.TASK_IS_NOT_COMPLETE, false)]
    [ValueDropdown("subTaskIds")]
    public string subTaskId;

    private GlobalState globalState;


    public void Init(Interactable interactable)
    {
        this.globalState = interactable.sceneCtrl.globalState;
    }

    public bool IsValid()
    {
        Objective objective = this.globalState.objectives[this.objectiveId];

        switch (this.type)
        {
            case ObjectiveConditionType.OBJECTIVE_IS_COMPLETE:
                return objective.completed;
            case ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE:
                return !objective.completed;
            case ObjectiveConditionType.TASK_IS_COMPLETE:
                return objective.tasks[this.taskId].completed;
            case ObjectiveConditionType.TASK_IS_NOT_COMPLETE:
                return !objective.tasks[this.taskId].completed;
            case ObjectiveConditionType.SUB_TASK_IS_COMPLETE:
                return objective.tasks[this.taskId].subTasks.Find(st => (st.id == this.subTaskId)).completed;
            case ObjectiveConditionType.SUB_TASK_IS_NOT_COMPLETE:
                return !objective.tasks[this.taskId].subTasks.Find(st => (st.id == this.subTaskId)).completed;
            case ObjectiveConditionType.SUB_TASK_CAN_BE_COMPLETED:
                List<SubTask> subTasks = objective.tasks[this.taskId].subTasks;
                int subTaskIndex = subTasks.FindIndex(st => (st.id == this.subTaskId));
                return
                (
                    // should be current objective
                    (this.objectiveId == this.globalState.currentObjective) &&
                    // all required subTasks before should be complete
                    subTasks.Take(subTaskIndex).All(st => (st.optional || st.completed)) &&
                    // no subTasks after should be complete
                    subTasks.Skip(subTaskIndex).All(st => !st.completed)
                );
            default:
                return false;
        }
    }
}

public enum ObjectiveConditionType
{
    OBJECTIVE_IS_COMPLETE,
    OBJECTIVE_IS_NOT_COMPLETE,
    TASK_IS_COMPLETE,
    TASK_IS_NOT_COMPLETE,
    SUB_TASK_IS_COMPLETE,
    SUB_TASK_IS_NOT_COMPLETE,
    SUB_TASK_CAN_BE_COMPLETED,
}
