using System;


public class ReachableCondition : ICondition
{
    private SceneController sceneCtrl;
    private IDataInteractable data;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
        this.data = interactable.data;
    }

    public bool IsValid()
    {
        Func<Tile, bool> neighbourFilter = (t) => (!t.isBlocked && t.isVisible);
        Predicate<Tile> reachableCheck = (t) => (t.FindPathFrom(this.sceneCtrl.player.tile, neighbourFilter) != null);
        return this.data.neighbourTiles.Exists(reachableCheck);
    }
}
