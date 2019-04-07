using System.Collections;


public interface ITurnActionReaction
{
    void Init(GlobalController globalCtrl);
    IEnumerator React();
}