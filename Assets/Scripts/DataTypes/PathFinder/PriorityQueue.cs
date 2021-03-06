using System.Collections.Generic;
using System.Linq;

class PriorityQueue<P, V>
{
    public bool isNotEmpty { get { return this.list.Any(); } }

    private readonly SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();


    public void Enqueue(P priority, V value)
    {
        Queue<V> queue;
        if (!list.TryGetValue(priority, out queue))
        {
            queue = new Queue<V>();
            list.Add(priority, queue);
        }
        queue.Enqueue(value);
    }

    public V Dequeue()
    {
        // will throw if there isn't any first element!
        KeyValuePair<P, Queue<V>> pair = list.First();
        V value = pair.Value.Dequeue();
        // nothing left of the top priority.
        if (pair.Value.Count == 0)
        {
            list.Remove(pair.Key);
        }

        return value;
    }
}
