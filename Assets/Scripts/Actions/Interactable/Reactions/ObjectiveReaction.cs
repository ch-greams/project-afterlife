using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;


public class ObjectiveReaction : IInteractableReaction
{
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
        this.objectiveManager.CompleteSubTask(this.objective.id, this.taskId, this.subTaskId);

        yield return null;
    }
}
