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

    public GameObject obj;

    [OdinSerialize]
    public IEnumerable<Tile> allNeighbours { get; set; }

    public IEnumerable<Tile> _activeNeighbours { get { return this.allNeighbours.Where(t => (!t.isBlocked && t.isVisible)); } }
    public IEnumerable<Tile> _walkableNeighbours { get { return this.allNeighbours.Where(t => (!t.isBlocked)); } }

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


    public List<Point> GetTiles(float range, bool isActive)
    {
        List<Point> result = new List<Point>();

        if (range > 0)
        {
            HashSet<Tile> closedSet = new HashSet<Tile>();
            Queue<Tile> openSet = new Queue<Tile>();

            this.UpdateOpenSet(isActive, closedSet, ref openSet);

            closedSet.Add(this);

            while (openSet.Count > 0)
            {
                Tile tileToCheck = openSet.Dequeue();
                Path<Tile> path = tileToCheck.FindPathFrom(this, isActive);

                closedSet.Add(tileToCheck);
                if (path.totalCost <= range)
                {
                    result.Add(tileToCheck.point);

                    tileToCheck.UpdateOpenSet(isActive, closedSet, ref openSet);
                }
            }
        }

        return result;
    }


    public Path<Tile> FindPathFrom(Tile startTile, bool isActive)
    {
        HashSet<Tile> closed = new HashSet<Tile>();
        PriorityQueue<double, Path<Tile>> queue = new PriorityQueue<double, Path<Tile>>();
        queue.Enqueue(0, new Path<Tile>(startTile));

        while (!queue.isEmpty)
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

            IEnumerable<Tile> neighbours = isActive ? path.lastStep._activeNeighbours : path.lastStep.allNeighbours;

            foreach (Tile tile in neighbours)
            {
                double distance = path.lastStep.point.DistanceTo(tile.point);
                Path<Tile> newPath = path.AddStep(tile, distance);
                queue.Enqueue(newPath.totalCost + tile.point.EstimateTo(this.point), newPath);
            }
        }

        return null;
    }


    public Tile GetTileByDirection(TileDirection direction)
    {
        switch (direction)
        {
            case TileDirection.Up:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(1, 0)) == tile.point);
            case TileDirection.Right:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(0, 1)) == tile.point);
            case TileDirection.Down:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(-1, 0)) == tile.point);
            case TileDirection.Left:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(0, -1)) == tile.point);

            case TileDirection.UpLeft:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(1, -1)) == tile.point);
            case TileDirection.UpRight:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(1, 1)) == tile.point);
            case TileDirection.DownRight:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(-1, 1)) == tile.point);
            case TileDirection.DownLeft:
                return this._activeNeighbours.FirstOrDefault(tile => (this.point + new Point(-1, -1)) == tile.point);

            case TileDirection.Undefined:
            default:
                return this;
        }
    }

    // TODO: Change return type to Path<Tile> ?
    public List<Tile> GetTilesByDirection(Point directionPoint, float range)
    {
        const int weaponRange = 5; // range

        Point deltaPoint = directionPoint - this.point;
        // Debug.Log("deltaPoint = " + deltaPoint);

        Tile currentTile = this._walkableNeighbours.FirstOrDefault(tile => tile.point == directionPoint);
        List<Tile> rayPath = new List<Tile>(){ currentTile };

        for (int step = 0; step < (weaponRange - 1); step++)
        {
            currentTile = currentTile._walkableNeighbours
                .FirstOrDefault(tile => tile.point == (currentTile.point + deltaPoint));

            // TODO: Check for null

            rayPath.Add(currentTile);
        }

        return rayPath;
    }

    private void UpdateOpenSet(bool isActive, HashSet<Tile> closedSet, ref Queue<Tile> openSet)
    {
        IEnumerable<Tile> neighboursToCheck = isActive ? this._activeNeighbours : this.allNeighbours;

        foreach (Tile neighbour in neighboursToCheck)
        {
            if (!closedSet.Contains(neighbour) && !openSet.Contains(neighbour))
            {
                openSet.Enqueue(neighbour);
            }
        }
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

// public enum TileState
// {
//     Disabled,
//     Hidden,
//     Active,
// }

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
