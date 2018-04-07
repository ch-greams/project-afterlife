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
    public TileState state;
    public GameObject obj;

    [OdinSerialize]
    public IEnumerable<Tile> allNeighbours { get; set; }


    public static Tile CreateInstance(Point point, TileState state, GameObject obj)
    {
        Tile tile = SerializedScriptableObject.CreateInstance<Tile>();
        tile.Init(point, state, obj);
        return tile;
    }

    public void Init(Point point, TileState state, GameObject obj)
    {
        this.point = point;
        this.state = state;
        this.obj = obj;
        base.name = obj.name;
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
            if (grid.ContainsKey(nPoint)) {
                neighbours.Add(grid[nPoint]);
            }
        }

        this.allNeighbours = neighbours;
    }

    public List<Tile> GetTiles(float range, TileState tileState)
    {
        if (range == 0)
        {
            return new List<Tile>();
        }

        HashSet<Tile> closedSet = new HashSet<Tile>();
        Queue<Tile> openSet = new Queue<Tile>();
        List<Tile> result = new List<Tile>();

        IEnumerable<Tile> neighbours = tileState == TileState.Active
            ? this.allNeighbours.Where(tile => (tile.state == TileState.Active))
            : this.allNeighbours;

        foreach (Tile neighbour in neighbours)
        {
            openSet.Enqueue(neighbour);
        }

        closedSet.Add(this);

        while (openSet.Count > 0)
        {
            Tile tileToCheck = openSet.Dequeue();
            Path<Tile> path = tileToCheck.FindPathFrom(this, tileState);

            closedSet.Add(tileToCheck);
            if (path.totalCost <= range)
            {
                result.Add(tileToCheck);

                IEnumerable<Tile> neighbourstoCheck = tileState == TileState.Active
                    ? tileToCheck.allNeighbours.Where(tile => (tile.state == TileState.Active))
                    : tileToCheck.allNeighbours;

                foreach (Tile tile in neighbourstoCheck)
                {
                    if (!closedSet.Contains(tile) && !openSet.Contains(tile))
                    {
                        openSet.Enqueue(tile);
                    }
                }
            }
        }
        return result;
    }

    public Path<Tile> FindPathFrom(Tile startTile, TileState tileState)
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

            IEnumerable<Tile> neighbours = tileState == TileState.Active
                ? path.lastStep.allNeighbours.Where(tile => (tile.state == TileState.Active))
                : path.lastStep.allNeighbours;

            foreach (Tile tile in neighbours)
            {
                double distance = path.lastStep.point.DistanceTo(tile.point);
                Path<Tile> newPath = path.AddStep(tile, distance);
                queue.Enqueue(newPath.totalCost + tile.point.EstimateTo(this.point), newPath);
            }
        }

        return null;
    }
}

[Serializable]
public class TileSimple {
    public Point point;
    public TileState state;


    public TileSimple()
    {
        this.point = new Point();
        this.state = TileState.Disabled;
    }

    public TileSimple(Tile tile)
    {
        this.point = tile.point;
        this.state = tile.state;
    }

    public TileSimple(Point point, TileState state)
    {
        this.point = point;
        this.state = state;
    }


    public TileSimple UpdateState(TileState state)
    {
        this.state = state;
        return this;
    }
}

public enum TileState
{
    Disabled,
    Hidden,
    Active,
}
