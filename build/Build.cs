using System;
using System.Collections.Generic;



public class Vector2i
{
	public int x;
	public int y;

	public Vector2i() { }

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static Vector2i operator +(Vector2i c1, Vector2i c2)
	{
		return new Vector2i(c1.x + c2.x, c1.y + c2.y);
	}

	public static Vector2i operator -(Vector2i c1, Vector2i c2)
	{
		return new Vector2i(c1.x - c2.x, c1.y - c2.y);
	}

	public static Vector2i operator *(Vector2i c1, int c2)
	{
		return new Vector2i(c1.x * c2, c1.y * c2);
	}

	public static Vector2i operator *(int c1, Vector2i c2)
	{
		return new Vector2i(c1 * c2.x, c1 * c2.y);
	}

	// allow callers to initialize
	public int this[int idx]
	{
		get { return idx == 0 ? x : y; }
		set
		{
			switch (idx)
			{
				case 0:
					x = value;
					break;
				default:
					y = value;
					break;
			}
		}
	}

	public double magnitude
	{
		get { return Math.Sqrt(x * x + y * y); }
	}

	public override bool Equals(System.Object obj)
	{
		// If parameter is null return false.
		if (obj == null)
		{
			return false;
		}

		// If parameter cannot be cast to Point return false.
		Vector2i p = obj as Vector2i;
		if ((System.Object)p == null)
		{
			return false;
		}

		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}

	public bool Equals(Vector2i p)
	{
		// If parameter is null return false:
		if ((object)p == null)
		{
			return false;
		}

		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}

	public static bool operator ==(Vector2i a, Vector2i b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}

		// Return true if the fields match:
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(Vector2i a, Vector2i b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public override string ToString()
	{
		return "Vector2i {" + x + ", " + y + "}";
	}
}



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



public interface IPathfinder<State, Transition>
{
	float Heuristic(State fromLocation, State toLocation);
	List<Transition> Expand(State position, State toState);
	double PathCost(State fromLocation, Transition transition);
	State ApplyTransition(State location, Transition transition);
}




class AStarNode<State, Transition>
{
	public AStarNode<State, Transition> parent;
	public State state;
	public Transition transition;
	public float g; // cost
	public float f; // estimate

	public AStarNode(AStarNode<State, Transition> parent, float g, float f, State state, Transition transition)
	{
		this.parent = parent;
		this.g = g;
		this.f = f;
		this.state = state;
		this.transition = transition;
	}

	public override string ToString()
	{
		return "Node {f:" + f + ", g:" + g + ", h:" + H + ", state: " + state + " transition: " + transition + "}";
	}

	public float H
	{
		get
		{
			return f - g;
		}
	}
}

class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
{
	public int Compare(T x, T y)
	{
		return y.CompareTo(x);
	}
}

public class AStarPathfinder<State, Transition>
{
	// A* Pathfinding Algorithm implementation

	private IPathfinder<State, Transition> map;

	public AStarPathfinder(IPathfinder<State, Transition> _map)
	{
		map = _map;
	}

	public List<Transition> getShortestPath(State fromState, State toState, Boolean collision = false)
	{
		AStarNode<State, Transition> bestNode = null;
		PriorityQueue<float, AStarNode<State, Transition>> openList = new PriorityQueue<float, AStarNode<State, Transition>>();
		Dictionary<State, AStarNode<State, Transition>> openListDictionary = new Dictionary<State, AStarNode<State, Transition>>();
		HashSet<State> closedSet = new HashSet<State>();

		AStarNode<State, Transition> startNode = CreateSearchNode(null, default(Transition), fromState, toState); // Create node for origin

		openList.Enqueue(startNode, 0);
		openListDictionary.Add(fromState, startNode); // Insert the node in the open list

		while (!openList.IsEmpty)
		{
			AStarNode<State, Transition> node = openList.Dequeue(); // Get lowest score node from open list
			openListDictionary.Remove(node.state);

			if (bestNode == null || bestNode.H > node.H)
			{
				bestNode = node;
			}

			if (node.state.Equals(toState)) // If this node is the final one, build and return solution
			{
				return BuildSolution(node);
			}

			closedSet.Add(node.state); // Add this node to the closed set

			foreach (Transition transition in map.Expand(node.state, toState)) // For every node reachable from this node (transitions)
			{
				State child = map.ApplyTransition(node.state, transition); // Get following state

				AStarNode<State, Transition> openListNode = null;
				bool isNodeInFrontier = openListDictionary.TryGetValue(child, out openListNode); // Gets node for state if it was already in the open list

				if (!closedSet.Contains(child) && !isNodeInFrontier) // If following state isn't in the closed list and is not in the open list too
				{
					AStarNode<State, Transition> searchNode = CreateSearchNode(node, transition, child, toState);

					openList.Enqueue(searchNode, searchNode.f);
					openListDictionary.Add(searchNode.state, searchNode);
				}
				else if (isNodeInFrontier) // Replaces node score if it's lower
				{
					AStarNode<State, Transition> searchNode = CreateSearchNode(node, transition, child, toState);

					if (openListNode.f > searchNode.f)
					{
						openList.Replace(openListNode, openListNode.f, searchNode.f);
					}
				}
			}
		}

		if (collision)
		{
			return BuildSolution(bestNode);
		}

		return null;
	}

	private AStarNode<State, Transition> CreateSearchNode(AStarNode<State, Transition> node, Transition transition, State child, State toState)
	{
		if (node != null)
		{
			float cost = (float)map.PathCost(node.state, transition);
			float heuristic = map.Heuristic(child, toState);

			return new AStarNode<State, Transition>(node, node.g + cost, node.g + cost + heuristic, child, transition);
		}
		else
		{
			float heuristic = map.Heuristic(child, toState);

			return new AStarNode<State, Transition>(node, 0, heuristic, child, transition);
		}
	}

	private List<Transition> BuildSolution(AStarNode<State, Transition> searchNode)
	{
		List<Transition> list = new List<Transition>();

		while (searchNode != null)
		{
			if ((searchNode.transition != null) && (!searchNode.transition.Equals(default(Transition))))
			{
				list.Insert(0, searchNode.transition);
			}

			searchNode = searchNode.parent;
		}

		return list;
	}
}

class Hero
{
    public int id;
    public int x;
    public int y;
    public int life;
    public int gold;
    public int myHeroDistance;
    public int spawnX;
    public int spawnY;
}

class Mine
{
    public int id;
    public int x;
    public int y;
    public int myHeroDistance;
}

class Tavern
{
    public int x;
    public int y;
    public int myHeroDistance;
}




class Core : IPathfinder<Vector2i, Vector2i>
{
    AStarPathfinder<Vector2i, Vector2i> pathfinder;
    public Hero myHero = null;
    public List<Hero> heroes = new List<Hero>();
    public List<Mine> mines = new List<Mine>();
    public List<Tavern> taverns = new List<Tavern>();
    public List<List<bool>> map = new List<List<bool>>();
    public Tavern nearestTavern;
    public Mine nearestUnclaimedMine;
    public int size;
    public int myId;

    private int heroCount = 0;
    private int mineCount = 0;

    List<Vector2i> transitions = new List<Vector2i>(4);

    public Core()
    {
        pathfinder = new AStarPathfinder<Vector2i, Vector2i>(this);

        transitions.Add(new Vector2i(1, 0));
        transitions.Add(new Vector2i(0, 1));
        transitions.Add(new Vector2i(-1, 0));
        transitions.Add(new Vector2i(0, -1));
    }

    public void StartTurn()
    {
        heroCount = 0;
        mineCount = 0;
        nearestTavern = null;
        nearestUnclaimedMine = null;
    }

    public void UpdateEntity(string entityType, int id, int x, int y, int life, int gold)
    {
        if (entityType == "HERO")
        {
            Hero hero = heroes[heroCount++];

            hero.id = id;
            hero.x = x;
            hero.y = y;
            hero.life = life;
            hero.gold = gold;

            if (hero.id == myId)
            {
                myHero = hero;
                hero.myHeroDistance = -1;

                foreach (Tavern tavern in taverns)
                {
                    List<Vector2i> path = pathfinder.getShortestPath(new Vector2i(myHero.x, myHero.y), new Vector2i(tavern.x, tavern.y));
                    tavern.myHeroDistance = path.Count - 1;

                    if (nearestTavern == null || tavern.myHeroDistance < nearestTavern.myHeroDistance)
                    {
                        nearestTavern = tavern;
                    }
                }
            }
            else
            {
                List<Vector2i> path = pathfinder.getShortestPath(new Vector2i(myHero.x, myHero.y), new Vector2i(hero.x, hero.y));
                hero.myHeroDistance = path.Count - 1;
            }
        }
        else if (entityType == "MINE")
        {
            Mine mine = mines[mineCount++];

            mine.id = id;
            mine.x = x;
            mine.y = y;

            List<Vector2i> path = pathfinder.getShortestPath(new Vector2i(myHero.x, myHero.y), new Vector2i(mine.x, mine.y));
            mine.myHeroDistance = path.Count - 1;

            if (
                mine.id != myId &&
                (
                    nearestUnclaimedMine == null ||
                    mine.myHeroDistance < nearestUnclaimedMine.myHeroDistance
                )
            )
            {
                nearestUnclaimedMine = mine;
            }
        }
    }

    public string GetAction()
    {
        // (X) Go to nearest tavern if all mines claimed and heal when life <= 75
        // (X) Next to tavern and need healing, heal
        // ( ) Avoid nearby enemies when they can kill me
        // ( ) Avoid enemies near taverns
        // ( ) Avoid stepping into another player's spawn if this player can die in his next turn (life < 20 * nearby enemies)
        // (X) If needs healing, seek tavern
        // ( ) Kill nearby enemies with at least 1 mine
        // ( ) Chase enemies with more than 1/4 of mines

        if (
            myHero.gold >= 2 &&
            (
                (
                    myHero.life <= 20
                ) ||
                (
                    nearestTavern.myHeroDistance == 0 &&
                    myHero.life <= 75
                ) ||
                (
                    nearestUnclaimedMine != null &&
                    myHero.life - nearestUnclaimedMine.myHeroDistance <= 20
                )
            )
        )
        {
            return "MOVE " + nearestTavern.x + " " + nearestTavern.y;
        }
        else
        {
            if (nearestUnclaimedMine == null)
            {
                if (nearestTavern.myHeroDistance == 0)
                {
                    return "WAIT";
                }
                else
                {
                    return "MOVE " + nearestTavern.x + " " + nearestTavern.y;
                }
            }
            else
            {
                return "MOVE " + nearestUnclaimedMine.x + " " + nearestUnclaimedMine.y;
            }
        }
    }

    public float Heuristic(Vector2i fromLocation, Vector2i toLocation)
    {
        Vector2i res = fromLocation - toLocation;
        return Math.Abs(res.x) + Math.Abs(res.y); // manhatten-distance
    }

    public List<Vector2i> Expand(Vector2i state, Vector2i toState)
    {
        List<Vector2i> res = new List<Vector2i>();

        foreach (Vector2i transition in transitions)
        {
            Vector2i newState = ApplyTransition(state, transition);

            if (
                (newState.x >= 0) &&
                (newState.y >= 0) &&
                (newState.x < size) &&
                (newState.y < size) &&
                (
                    newState.Equals(toState) || map[newState.y][newState.x]
                )
            )
            {
                res.Add(transition);
            }
        }

        return res;
    }

    public double PathCost(Vector2i fromLocation, Vector2i transition)
    {
        return transition.magnitude;
    }

    public Vector2i ApplyTransition(Vector2i state, Vector2i transition)
    {
        return state + transition;
    }
}




class Player
{
    static void Main(string[] args)
    {
        Core core = new Core();

        int size = int.Parse(Console.ReadLine());

        core.size = size;

        for (int i = 0; i < size; i++)
        {
            string line = Console.ReadLine();

            core.map.Add(new List<bool>());

            for (int j = 0; j < line.Length; j++)
            {
                char c = line[j];

                switch(c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    {
                        Hero hero = new Hero();
                        hero.spawnX = j;
                        hero.spawnY = i;

                        core.heroes.Add(hero);
                        break;
                    }
                    case 'T':
                    {
                        Tavern tavern = new Tavern();
                        tavern.y = i;
                        tavern.x = j;

                        core.taverns.Add(tavern);
                        break;
                    }
                    case 'M':
                        core.mines.Add(new Mine());
                        break;
                }

                core.map[i].Add(c == '.' || c == '0' || c == '1' || c == '2' || c == '3');
            }
        }

        core.myId = int.Parse(Console.ReadLine()); // ID of your hero
        
        // game loop
        while (true)
        {
            core.StartTurn();

            int entityCount = int.Parse(Console.ReadLine()); // the number of entities

            for (int i = 0; i < entityCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                string entityType = inputs[0]; // HERO or MINE
                int id = int.Parse(inputs[1]); // the ID of a hero or the owner of a mine
                int x = int.Parse(inputs[2]); // the x position of the entity
                int y = int.Parse(inputs[3]); // the y position of the entity
                int life = int.Parse(inputs[4]); // the life of a hero (-1 for mines)
                int gold = int.Parse(inputs[5]); // the gold of a hero (-1 for mines)

                core.UpdateEntity(entityType, id, x, y, life, gold);
            }

            Console.WriteLine(core.GetAction());
        }
    }
}