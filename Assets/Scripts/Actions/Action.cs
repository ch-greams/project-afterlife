using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;


public class Action
{
    public string name;

    [GUIColor(0.85F, 1F, 1F, 1F)]
    [ValidateInput("ValidateObjectiveConditions", "At least one ObjectiveCondition has conflicting values in ID and Objective fields.")]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<ICondition> conditions = new List<ICondition>();

    // TODO: Use for List<ICondition> OR List<ICondition>
    // [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    // public List<List<ICondition>> conditionCollection = new List<List<ICondition>>();


    [GUIColor(1F, 0.85F, 0.85F, 1F)]
    [ValidateInput("ValidateObjectiveReactions", "At least one ObjectiveReaction has conflicting values in ID and Objective fields.")]
    [ListDrawerSettings(Expanded = false)]
    public List<IReaction> reactions = new List<IReaction>();


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
        foreach (IReaction reaction in this.reactions)
        {
            yield return reaction.React();
        }
    }


    private bool ValidateObjectiveConditions(List<ICondition> conditions)
    {
        List<ObjectiveCondition> ocs = (conditions != null)
            ? conditions
                .Where(cond => cond.GetType().Name == "ObjectiveCondition")
                .Select(cond => cond as ObjectiveCondition).ToList()
            : new List<ObjectiveCondition>();

        return ocs.All(oc => {
            switch (oc.type)
            {
                case ObjectiveConditionType.OBJECTIVE_IS_COMPLETE:
                case ObjectiveConditionType.OBJECTIVE_IS_NOT_COMPLETE:
                    return true;
                default:
                    return (oc.objective != null) && (oc.objective.id == oc.objectiveId);
            }
        });
    }

    private bool ValidateObjectiveReactions(List<IReaction> conditions)
    {
        List<ObjectiveReaction> ors = (conditions != null)
            ? conditions
                .Where(cond => cond.GetType().Name == "ObjectiveReaction")
                .Select(cond => cond as ObjectiveReaction).ToList()
            : new List<ObjectiveReaction>();

        return ors.All(or => (or.objective != null) && (or.objective.id == or.objectiveId));
    }
}
