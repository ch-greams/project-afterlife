using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class EnemyAction
{
    public string name;

    [GUIColor(0.85F, 1F, 1F, 1F)]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<IEnemyActionCondition> conditions = new List<IEnemyActionCondition>();

    [GUIColor(1F, 0.85F, 0.85F, 1F)]
    [ListDrawerSettings(Expanded = false)]
    public List<IEnemyActionReaction> reactions = new List<IEnemyActionReaction>();


    public EnemyAction() { }

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
        foreach (IEnemyActionReaction reaction in this.reactions)
        {
            yield return reaction.React();
        }
    }
}
