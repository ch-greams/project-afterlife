using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class Tile
{
    public bool passable;
    public Point point;
    public GameObject obj;

    public IEnumerable<Tile> allNeighbours { get; set; }
    public IEnumerable<Tile> neighbours { get { return allNeighbours.Where(tile => tile.passable); } }

    public Dictionary<Point, bool> shifts;

    public Tile(Point point, bool passable, GameObject obj, Dictionary<Point, bool> shifts)
    {
        this.point = point;
        this.passable = passable;
        this.obj = obj;

        this.shifts = new Dictionary<Point, bool>(shifts);
    }

    /// <summary>
    /// Generates AllNeighbours list
    /// </summary>
    public void FindNeighbours(Dictionary<Point, Tile> board)
    {
        List<Tile> neighbours = new List<Tile>();

        foreach (Point shift in this.shifts.Where(kvp => kvp.Value).Select(kvp => kvp.Key))
        {
            neighbours.Add(board[this.point + shift]);
        }

        this.allNeighbours = neighbours;
    }
    public List<Tile> GetTiles(float range, bool passable)
    {
        if (range == 0)
        {
            return new List<Tile>();
        }

        HashSet<Tile> closedSet = new HashSet<Tile>();
        Queue<Tile> openSet = new Queue<Tile>();
        List<Tile> result = new List<Tile>();

        IEnumerable<Tile> neighbours = passable ? this.neighbours : this.allNeighbours;

        foreach (Tile neighbour in neighbours)
        {
            openSet.Enqueue(neighbour);
        }

        closedSet.Add(this);

        while (openSet.Count > 0)
        {
            Tile tileToCheck = openSet.Dequeue();

            Path<Tile> path = tileToCheck.FindPathFrom(this, passable);

            closedSet.Add(tileToCheck);
            if (path.totalCost <= range)
            {
                result.Add(tileToCheck);

                IEnumerable<Tile> neighbourstoCheck = passable ? tileToCheck.neighbours : tileToCheck.allNeighbours;
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

    public Path<Tile> FindPathFrom(Tile startTile, bool passable)
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

            IEnumerable<Tile> neighbours = passable ? path.lastStep.neighbours : path.lastStep.allNeighbours;

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
