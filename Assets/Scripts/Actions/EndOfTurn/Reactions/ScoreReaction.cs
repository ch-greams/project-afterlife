using System.Collections;


public class ScoreReaction : IEndOfTurnReaction
{
    public ScoreReactionType type = ScoreReactionType.Undefined;
    public int value = 0;

    private StatsManager statsManager;


    public void Init(GlobalController globalCtrl)
    {
        this.statsManager = globalCtrl.statsManager;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case ScoreReactionType.IncrementScoreValue:
                this.statsManager.IncrementScore(this.value);
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
