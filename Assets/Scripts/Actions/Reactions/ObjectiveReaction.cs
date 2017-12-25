using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class ObjectiveReaction : IReaction
{
    public ObjectiveReactionType type;

    [ValidateInput("ValidateObjectiveId", "Objective ID doesn't match, please select correct Objective.")]
    public ObjectiveId objectiveId;

    // NOTE: Editor use only, don't use this variable in Init/IsValid methods
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

    [ValueDropdown("subTaskIds")]
    public string subTaskId;

    private GlobalController globalCtrl;


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }

    public IEnumerator React()
    {
        Objective objective = this.globalCtrl.globalState.objectives[this.objectiveId];
        objective.Complete(this.taskId, this.subTaskId);
        if (objective.completed)
        {
            this.globalCtrl.globalState.currentObjective = objective.nextObjectives[0].id;
        }
        this.globalCtrl.objectiveManager.UpdateObjective();

        yield return null;
    }


    private bool ValidateObjectiveId(ObjectiveId objectiveId)
    {
        return (this.objective != null) && (this.objective.id == objectiveId);
    }

    private bool ValidateObjective(Objective objective)
    {
        return (objective != null) && (objective.id == this.objectiveId);
    }
}

public enum ObjectiveReactionType
{
    COMPLETE_SUB_TASK,
}
