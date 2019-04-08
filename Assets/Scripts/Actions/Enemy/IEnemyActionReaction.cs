using System.Collections;


public interface IEnemyActionReaction
{
    void Init(GlobalController globalCtrl);
    IEnumerator React();
}