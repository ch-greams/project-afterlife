using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;


[Serializable]
public class Tile : SerializedScriptableObject
{
    public Point point;

    public bool isVisible = false;
    public bool isBlocked = false;
    public bool isActive = false;
    public bool isSelected = false;

    public GameObject obj;

    [OdinSerialize]
    public IEnumerable<Tile> allNeighbours { get; set; }


    public Tile Init(Point point, bool isVisible, bool isBlocked, GameObject obj)
    {
        this.point = point;
        this.isVisible = isVisible;
        this.isBlocked = isBlocked;
        this.obj = obj;
        base.name = obj.name;

        return this;
    }

    public static Tile CreateInstance(Point point, bool isVisible, bool isBlocked, GameObject obj)
    {
        return SerializedScriptableObject.CreateInstance<Tile>().Init(point, isVisible, isBlocked, obj);
    }

    /// <summary>
    /// Generates AllNeighbours list
    /// </summary>
    public void FindNeighbours(Dictionary<Point, Tile> grid)
    {
        List<Point> shifts = new List<Point>()
        {
            new Point(0, 1),  new Point(1, 1),   new Point(1, 0),  new Point(1, -1),
            new Point(0, -1), new Point(-1, -1), new Point(-1, 0), new Point(-1, 1),
        };

        List<Tile> neighbours = new List<Tile>();

        foreach (Point shift in shifts)
        {
            Point nPoint = this.point + shift;
            if (grid.ContainsKey(nPoint))
            {
                neighbours.Add(grid[nPoint]);
            }
        }

        this.allNeighbours = neighbours;
    }

    public HashSet<Point> GetTiles(float range, Func<Tile, bool> neighbourFilter)
    {
        HashSet<Point> result = new HashSet<Point>();

        HashSet<(Tile tile, double cost)> tilesToCheck = new HashSet<(Tile tile, double cost)>();
        tilesToCheck.Add((tile: this, cost: 0));

        while (tilesToCheck.Any())
        {
            (Tile tile, double cost) currentTile = tilesToCheck.First();
            tilesToCheck.Remove(currentTile);

            IEnumerable<Tile> neighbourTiles = currentTile.tile.allNeighbours
                .Where(neighbourFilter)
                .Where((tile) => ((tile.point.DistanceTo(currentTile.tile.point) + currentTile.cost) <= range))
                .Where((tile) => !result.Any((point) => (tile.point == point)));

            IEnumerable<(Tile tile, double cost)> neighbourTuples = neighbourTiles
                .Select(tile => (
                    tile: tile,
                    cost: currentTile.cost + tile.point.DistanceTo(currentTile.tile.point)
                ));

            tilesToCheck.UnionWith(neighbourTuples);
            tilesToCheck = new HashSet<(Tile tile, double cost)>(tilesToCheck.OrderBy(t => t.cost));

            result.UnionWith(neighbourTiles.Select((tile) => tile.point));
        }

        return result;
    }

    public Path<Tile> FindPathFrom(Tile startTile, Func<Tile, bool> neighbourFilter)
    {
        HashSet<Tile> closed = new HashSet<Tile>();
        PriorityQueue<double, Path<Tile>> queue = new PriorityQueue<double, Path<Tile>>();
        queue.Enqueue(0, new Path<Tile>(startTile));

        while (queue.isNotEmpty)
        {
            Path<Tile> path = queue.Dequeue();

            if (closed.Contains(path.lastStep))
            {
                continue;
            }
            if (path.lastStep.Equals(this))
            {
                return path;
            }

            closed.Add(path.lastStep);

            IEnumerable<Tile> neighbours = path.lastStep.allNeighbours.Where(neighbourFilter);

            foreach (Tile tile in neighbours)
            {
                double distance = path.lastStep.point.DistanceTo(tile.point);
                Path<Tile> newPath = path.AddStep(tile, distance);
                queue.Enqueue(newPath.totalCost + tile.point.EstimateTo(this.point), newPath);
            }
        }

        return null;
    }

    public Path<Tile> FindPathFrom(Tile startTile, Func<Tile, bool> neighbourFilter, float range)
    {
        Path<Tile> path = this.FindPathFrom(startTile, neighbourFilter);

        while ((path != null) && (path.totalCost > range))
        {
            path = path.previousSteps;
        }

        return path;
    }

    public Tile GetTileByDirection(TileDirection direction, Func<Tile, bool> filter)
    {
        IEnumerable<Tile> neighbours = this.allNeighbours.Where(filter);

        switch (direction)
        {
            case TileDirection.Up:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(1, 0)) == tile.point);
            case TileDirection.Right:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(0, 1)) == tile.point);
            case TileDirection.Down:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(-1, 0)) == tile.point);
            case TileDirection.Left:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(0, -1)) == tile.point);

            case TileDirection.UpLeft:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(1, -1)) == tile.point);
            case TileDirection.UpRight:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(1, 1)) == tile.point);
            case TileDirection.DownRight:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(-1, 1)) == tile.point);
            case TileDirection.DownLeft:
                return neighbours.FirstOrDefault(tile => (this.point + new Point(-1, -1)) == tile.point);

            case TileDirection.Undefined:
            default:
                return this;
        }
    }

    public Path<Tile> GetTilesByDirection(Point directionPoint, float range)
    {
        Point deltaPoint = directionPoint - this.point;

        Tile currentTile = this.allNeighbours.FirstOrDefault(tile => tile.point == directionPoint);
        Path<Tile> rayPath = new Path<Tile>(currentTile);

        for (int step = 0; step < (range - 1); step++)
        {
            currentTile = currentTile.allNeighbours
                .FirstOrDefault(tile => tile.point == (currentTile.point + deltaPoint));

            if (currentTile == null)
            {
                break;
            }
            
            rayPath = rayPath.AddStep(currentTile, new Point().DistanceTo(deltaPoint));
        }

        return rayPath;
    }

    public void RefreshTileState(bool isVisible, bool isBlocked, bool inEditor = false)
    {
        this.isVisible = isVisible;
        this.isBlocked = isBlocked;

        TileData tileData = this.obj.GetComponent<Interactable>().data as TileData;
        tileData.RefreshTileMaterial(this, inEditor);
    }

    public void RefreshTileState(bool isVisible, bool isBlocked, bool isActive, bool isSelected, bool inEditor = false)
    {
        this.isVisible = isVisible;
        this.isBlocked = isBlocked;

        this.isActive = isActive;
        this.isSelected = isSelected;

        TileData tileData = this.obj.GetComponent<Interactable>().data as TileData;
        tileData.RefreshTileMaterial(this, inEditor);
    }
}

[Serializable]
public class TileSimple
{
    public Point point;
    public bool isVisible = false;
    public bool isBlocked = false;


    public TileSimple(Tile tile)
    {
        this.point = tile.point;

        this.isVisible = tile.isVisible;
        this.isBlocked = tile.isBlocked;
    }

    public TileSimple(Point point, bool isVisible, bool isBlocked)
    {
        this.point = point;

        this.isVisible = isVisible;
        this.isBlocked = isBlocked;
    }
}

public enum TileDirection
{
    Undefined,
    Up,
    Right,
    Down,
    Left,
    UpLeft,
    UpRight,
    DownRight,
    DownLeft,
}
