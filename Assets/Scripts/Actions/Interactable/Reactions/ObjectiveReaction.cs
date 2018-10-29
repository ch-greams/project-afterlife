using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;


public class ObjectiveReaction : IInteractableReaction
{
    public ObjectiveReactionType type;

    public ObjectiveId objectiveId;

    // NOTE: Editor use only, don't use this variable in Init/IsValid methods
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
                ? this.objective.tasks[this.taskId].subTasks.Select(st => st.id).ToList()
                : new List<string>();
        }
    }

    [ValueDropdown("subTaskIds")]
    public string subTaskId;

    private ObjectiveManager objectiveManager;


    public void Init(Interactable interactable)
    {
        this.objectiveManager = interactable.sceneCtrl.globalCtrl.objectiveManager;
    }

    public IEnumerator React()
    {
        this.objectiveManager.CompleteSubTask(this.objectiveId, this.taskId, this.subTaskId);

        yield return null;
    }
}

public enum ObjectiveReactionType
{
    COMPLETE_SUB_TASK,
}
