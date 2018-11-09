﻿using System.Collections;


public class ScoreReaction : IEndOfTurnReaction
{
    public ScoreReactionType type = ScoreReactionType.Undefined;
    public int value = 0;

    private ScoreManager scoreManager;


    public void Init(GlobalController globalCtrl)
    {
        this.scoreManager = globalCtrl.scoreManager;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case ScoreReactionType.IncrementScoreValue:
                this.scoreManager.IncrementScore(this.value);
                break;
            case ScoreReactionType.Undefined:
            default:
                yield return null;
                break;
        }
    }
}

public enum ScoreReactionType
{
    Undefined,
    IncrementScoreValue,
}