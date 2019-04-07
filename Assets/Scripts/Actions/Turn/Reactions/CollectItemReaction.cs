using System.Collections;


public class CollectItemReaction : ITurnActionReaction
{
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        this.globalCtrl.sceneCtrl.player.TryCollectItem();

        yield return null;
    }
}
