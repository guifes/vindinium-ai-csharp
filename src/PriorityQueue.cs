using System.Collections.Generic;

public class PriorityQueue<P, V>
{
	private SortedDictionary<P, LinkedList<V>> list;

	public PriorityQueue(IComparer<P> comparer = null)
	{
		if (comparer != null)
		{
			list = new SortedDictionary<P, LinkedList<V>>(comparer);
		}
		else
		{
			list = new SortedDictionary<P, LinkedList<V>>();
		}
	}

	public void Enqueue(V value, P priority)
	{
		LinkedList<V> q;

		if (!list.TryGetValue(priority, out q))
		{
			q = new LinkedList<V>();
			list.Add(priority, q);
		}

		q.AddLast(value);
	}

	public V Dequeue()
	{
		// will throw exception if there isn’t any first element!
		SortedDictionary<P, LinkedList<V>>.KeyCollection.Enumerator enumerator = list.Keys.GetEnumerator();

		enumerator.MoveNext();

		P key = enumerator.Current;

		LinkedList<V> v = list[key];

		V res = v.First.Value;
		v.RemoveFirst();

		if (v.Count == 0) // nothing left of the top priority.
		{
			list.Remove(key);
		}

		return res;
	}

	public V Dequeue(out P priority)
	{
		// will throw exception if there isn’t any first element!
		SortedDictionary<P, LinkedList<V>>.KeyCollection.Enumerator enumerator = list.Keys.GetEnumerator();

		enumerator.MoveNext();

		P key = enumerator.Current;

		LinkedList<V> v = list[key];

		V res = v.First.Value;
		v.RemoveFirst();

		if (v.Count == 0) // nothing left of the top priority.
		{
			list.Remove(key);
		}

		priority = key;

		return res;
	}

	public void Replace(V value, P oldPriority, P newPriority)
	{
		LinkedList<V> v = list[oldPriority];
		v.Remove(value);

		if (v.Count == 0) // nothing left of the top priority.
		{
			list.Remove(oldPriority);
		}

		Enqueue(value, newPriority);
	}

	public bool IsEmpty
	{
		get
		{
			return list.Count == 0;
		}
	}

	public override string ToString()
	{
		string res = "";

		foreach (P key in list.Keys)
		{
			foreach (V val in list[key])
			{
				res += val + ", ";
			}
		}

		return res;
	}
}