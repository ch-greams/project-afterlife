

public interface IEndOfTurnCondition
{
    void Init(EndOfTurnActionState endOfTurnActionState);
    bool IsValid();
}
