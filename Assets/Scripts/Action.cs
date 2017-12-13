using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class Action
{
    public string name;
    [ListDrawerSettings(DraggableItems = false)]
    public List<ICondition> conditions = new List<ICondition>();
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
