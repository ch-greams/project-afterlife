using System.Collections.Generic;
using Sirenix.OdinInspector;


public class ObjectiveCondition : ICondition
{
    public ObjectiveConditionType type;

    [ValidateInput("RequiredObjectiveId", "Objective ID is required.")]
    [ValidateInput("ValidateObjectiveId", "Objective ID doesn't match, please select correct Objective.")]
    public ObjectiveId objectiveId;

    // NOTE: Editor use only, don't use this variable in Init/IsValid methods
    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_COMPLETE, false)]
    [HideIf("type", ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE, false)]
    [ValidateInput("ValidateObjective", "Objective ID doesn't match, please select correct Objective.")]
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
                ? new List<string>(this.objective.tasks[this.taskId].subTasks.Keys)
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
                return objective.tasks[this.taskId].subTasks[this.subTaskId].completed;
            case ObjectiveConditionType.SUB_TASK_IS_NOT_COMPLETE:
                return !objective.tasks[this.taskId].subTasks[this.subTaskId].completed;
            default:
                return false;
        }
    }

    private bool RequiredObjectiveId(ObjectiveId objectiveId)
    {
        return (objectiveId != ObjectiveId.Undefined);
    }

    private bool ValidateObjectiveId(ObjectiveId objectiveId)
    {
        switch (this.type)
        {
            case ObjectiveConditionType.OBJECTIVE_IS_COMPLETE:
            case ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE:
                return true;
            default:
                return (this.objective != null) && (this.objective.id == objectiveId);
        }
    }

    private bool ValidateObjective(Objective objective)
    {
        return (objective != null) && (objective.id == this.objectiveId);
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
}
