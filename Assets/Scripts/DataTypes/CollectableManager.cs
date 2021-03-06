﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class CollectableManager : SerializedMonoBehaviour
{
    public GameObject collectableItemPrefab;
    public GameObject collectionEffectPrefab;

    public Dictionary<Point, CollectableItem> collectableItems = new Dictionary<Point, CollectableItem>();


    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        // TODO: Load from sceneState
    }


    public void TryCollectItem(Point point)
    {
        if (this.collectableItems.ContainsKey(point))
        {
            this.collectableItems[point].Destroy();
            this.collectableItems.Remove(point);

            Vector3 sceneCtrlPosition = this.globalCtrl.sceneCtrl.transform.position;
            GameObject.Instantiate(
                original: this.collectionEffectPrefab,
                position: point.CalcWorldCoord(sceneCtrlPosition, 0.5F),
                rotation: this.collectableItemPrefab.transform.rotation
            );

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
        Vector3 tilePosition = tile.gameObject.transform.position;
        Vector3 itemPosition = new Vector3(tilePosition.x, 0.25F, tilePosition.z);

        GameObject obj = GameObject.Instantiate(this.collectableItemPrefab, itemPosition, Quaternion.identity);

        CollectableItem collectableItem = new CollectableItem("Item " + point, obj);

        this.collectableItems.Add(point, collectableItem);
    }
}
