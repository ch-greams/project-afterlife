using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class Action
{
    public string name;

    [GUIColor(0.85F, 1F, 1F, 1F)]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<ICondition> conditions = new List<ICondition>();

    // TODO: Use for List<ICondition> OR List<ICondition>
    // [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    // public List<List<ICondition>> conditionCollection = new List<List<ICondition>>();


    [GUIColor(1F, 0.85F, 0.85F, 1F)]
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
}
