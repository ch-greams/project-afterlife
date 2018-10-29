using System.Collections;


public interface IEndOfTurnReaction
{
    void Init(EndOfTurnActionState endOfTurnActionState);
    IEnumerator React();
}