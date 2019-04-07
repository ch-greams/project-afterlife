using System.Collections;


public interface IEndOfTurnReaction
{
    void Init(GlobalController globalCtrl);
    IEnumerator React();
}