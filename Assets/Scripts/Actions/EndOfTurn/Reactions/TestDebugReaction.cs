using System.Collections;
using UnityEngine;


public class TestDebugReaction : IEndOfTurnReaction
{
    public string logText = "Triggered";
    private EnemyKillConditionState enemyKillConditionState;


    public void Init(EndOfTurnActionState endOfTurnActionState)
    {
        this.enemyKillConditionState = endOfTurnActionState.enemyKillConditionState;
    }

    public IEnumerator React()
    {
        Debug.Log("TestDebugReaction.React(): " + this.logText);

        yield return null;
    }
}
