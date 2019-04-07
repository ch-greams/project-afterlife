using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class TurnAction : IComparable<TurnAction>
{
    public string name;
    public int index;

    [GUIColor(0.85F, 1F, 1F, 1F)]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<ITurnActionCondition> conditions = new List<ITurnActionCondition>();

    [GUIColor(1F, 0.85F, 0.85F, 1F)]
    [ListDrawerSettings(Expanded = false)]
    public List<ITurnActionReaction> reactions = new List<ITurnActionReaction>();


    public TurnAction() { }

    public void Init(GlobalController globalCtrl)
    {
        this.conditions.ForEach(condition => condition.Init(globalCtrl));
        this.reactions.ForEach(reaction => reaction.Init(globalCtrl));
    }

    public bool IsValid()
    {
        return this.conditions.TrueForAll(condition => condition.IsValid());
    }

    public IEnumerator React()
    {
        foreach (ITurnActionReaction reaction in this.reactions)
        {
            yield return reaction.React();
        }
    }

    public int CompareTo(TurnAction other)
    {
        return this.index - other.index;
    }
}
