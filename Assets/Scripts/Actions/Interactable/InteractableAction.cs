using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;


public class InteractableAction
{
    public string name;

    [GUIColor(0.85F, 1F, 1F, 1F)]
    [ValidateInput("ValidateObjectiveConditions", "At least one ObjectiveCondition has conflicting values in ID and Objective fields.")]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<IInteractableCondition> conditions = new List<IInteractableCondition>();

    [GUIColor(1F, 0.85F, 0.85F, 1F)]
    [ValidateInput("ValidateObjectiveReactions", "At least one ObjectiveReaction has conflicting values in ID and Objective fields.")]
    [ListDrawerSettings(Expanded = false)]
    public List<IInteractableReaction> reactions = new List<IInteractableReaction>();


    public InteractableAction() { }

    public void Init(Interactable interactable)
    {
        this.conditions.ForEach(condition => condition.Init(interactable));
        this.reactions.ForEach(reaction => reaction.Init(interactable));
    }

    public bool IsValid()
    {
        return this.conditions.TrueForAll(condition => condition.IsValid());
    }

    public IEnumerator React()
    {
        foreach (IInteractableReaction reaction in this.reactions)
        {
            yield return reaction.React();
        }
    }

    /// <summary>
    /// Used for property validation by Odin Inspector
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    private bool ValidateObjectiveConditions(List<IInteractableCondition> conditions)
    {
        List<ObjectiveCondition> ocs = (conditions != null)
            ? conditions
                .Where(cond => (cond != null) && (cond.GetType().Name == "ObjectiveCondition"))
                .Select(cond => cond as ObjectiveCondition).ToList()
            : new List<ObjectiveCondition>();

        return ocs.All(oc => {
            switch (oc.type)
            {
                case ObjectiveConditionType.ObjectiveIsComplete:
                case ObjectiveConditionType.ObjectiveIsNotComplete:
                    return true;
                default:
                    return (oc.objective != null);
            }
        });
    }

    /// <summary>
    /// Used for property validation by Odin Inspector
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    private bool ValidateObjectiveReactions(List<IInteractableReaction> conditions)
    {
        List<ObjectiveReaction> ors = (conditions != null)
            ? conditions
                .Where(cond => (cond != null) && (cond.GetType().Name == "ObjectiveReaction"))
                .Select(cond => cond as ObjectiveReaction).ToList()
            : new List<ObjectiveReaction>();

        return ors.All(or => (or.objective != null));
    }
}
