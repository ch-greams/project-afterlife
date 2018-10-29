using System.Collections.Generic;
using UnityEngine;


public class CollectableManager : IManagerWithEndOfTurnActions
{
    public GameObject collectableItemPrefab;

    public Dictionary<Point, CollectableItem> collectableItems = new Dictionary<Point, CollectableItem>();

    public List<EndOfTurnAction> endOfTurnActions { get { return this._endOfTurnActions; } }
    public List<EndOfTurnAction> _endOfTurnActions = new List<EndOfTurnAction>();


    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        foreach (EndOfTurnAction endOfTurnAction in this.endOfTurnActions)
        {
            endOfTurnAction.Init(this.globalCtrl);   
        }

        // TODO: Load from sceneState
    }


    public void TryCollectItem(Point point)
    {
        if (this.collectableItems.ContainsKey(point))
        {
            this.collectableItems[point].Destroy();
            this.collectableItems.Remove(point);

            Player player = this.globalCtrl.sceneCtrl.player;
            player.ChangeVisibleRange(player.visibleRange + 1);
        }
    }

    public void TrySpawnItem(Point point, string name)
    {
        if (!this.collectableItems.ContainsKey(point))
        {
            this.SpawnItem(point, name);
        }
    }

    public void SpawnItem(Point point, string name)
    {
        Tile tile = this.globalCtrl.sceneCtrl.tiles.Find(t => t.point == point);
        Vector3 position = tile.obj.transform.position;
        position.y = 0.25F;

        GameObject obj = GameObject.Instantiate(this.collectableItemPrefab, position, Quaternion.identity);

        CollectableItem collectableItem = new CollectableItem("Item " + point, obj);

        this.collectableItems.Add(point, collectableItem);
    }
}
