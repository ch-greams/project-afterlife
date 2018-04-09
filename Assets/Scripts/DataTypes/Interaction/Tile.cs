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

    public IEnumerable<Tile> activeNeighbours { get { return this.allNeighbours.Where(t => (t.state == TileState.Active)); } }


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
            if (grid.ContainsKey(nPoint))
            {
                neighbours.Add(grid[nPoint]);
            }
        }

        this.allNeighbours = neighbours;
    }

    public List<Point> GetTiles(float range, TileState tileState)
    {
        List<Point> result = new List<Point>();

        if (range > 0)
        {
            HashSet<Tile> closedSet = new HashSet<Tile>();
            Queue<Tile> openSet = new Queue<Tile>();

            this.UpdateOpenSet(tileState, closedSet, ref openSet);

            closedSet.Add(this);

            while (openSet.Count > 0)
            {
                Tile tileToCheck = openSet.Dequeue();
                Path<Tile> path = tileToCheck.FindPathFrom(this, tileState);

                closedSet.Add(tileToCheck);
                if (path.totalCost <= range)
                {
                    result.Add(tileToCheck.point);

                    tileToCheck.UpdateOpenSet(tileState, closedSet, ref openSet);
                }
            }
        }

        return result;
    }

    private void UpdateOpenSet(TileState tileState, HashSet<Tile> closedSet, ref Queue<Tile> openSet)
    {
        IEnumerable<Tile> neighboursToCheck = (
            (tileState == TileState.Active) ? this.activeNeighbours : this.allNeighbours
        );

        foreach (Tile neighbour in neighboursToCheck)
        {
            if (!closedSet.Contains(neighbour) && !openSet.Contains(neighbour))
            {
                openSet.Enqueue(neighbour);
            }
        }
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

            IEnumerable<Tile> neighbours = (
                (tileState == TileState.Active) ? path.lastStep.activeNeighbours : path.lastStep.allNeighbours
            );

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
public class TileSimple
{
    public Point point;
    public TileState state;


    public TileSimple(Tile tile)
    {
        this.point = tile.point;
        this.state = tile.state;
    }
}

public enum TileState
{
    Disabled,
    Hidden,
    Active,
}
