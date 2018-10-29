

public interface IEndOfTurnCondition
{
    void Init(GlobalController globalCtrl);
    bool IsValid();
}
