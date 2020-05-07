using System;
using System.Collections.Generic;

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

class AStarMovementNode<State, Transition>
{
	public State state;
	public int steps;
	
	public AStarMovementNode(int steps, State state)
	{
		this.steps = steps;
		this.state = state;
	}
	
	public override string ToString()
	{
		return "Node {steps:" + steps + ", state: " + state + "}";
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

		while(!openList.IsEmpty)
		{
			AStarNode<State, Transition> node = openList.Dequeue(); // Get lowest score node from open list
			openListDictionary.Remove(node.state);

			if(bestNode == null || bestNode.H > node.H)
			{
				bestNode = node;
			}

			if (node.state.Equals(toState)) // If this node is the final one, build and return solution
			{
				return BuildSolution(node);
			}

			closedSet.Add(node.state); // Add this node to the closed set

			foreach (Transition transition in map.Expand(node.state)) // For every node reachable from this node (transitions)
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
				else if(isNodeInFrontier) // Replaces node score if it's lower
				{
					AStarNode<State, Transition> searchNode = CreateSearchNode(node, transition, child, toState);

					if (openListNode.f > searchNode.f)
					{
						openList.Replace(openListNode, openListNode.f, searchNode.f);
					}
				}
			}
		}
		
		if(collision)
		{
			return BuildSolution(bestNode);
		}

		return null;
	}

	public HashSet<State> getMovementRange(State fromState, int movementRange, bool debug = false)
	{
		PriorityQueue<int, AStarMovementNode<State, Transition>> openList = new PriorityQueue<int, AStarMovementNode<State, Transition>>(new DescendingComparer<int>());
		Dictionary<State, AStarMovementNode<State, Transition>> openListDictionary = new Dictionary<State, AStarMovementNode<State, Transition>>();
		HashSet<State> closedSet = new HashSet<State>();
		
		AStarMovementNode<State, Transition> startNode = new AStarMovementNode<State, Transition>(map.StepsForState(fromState, movementRange), fromState);
		
		openList.Enqueue(startNode, 0);
		openListDictionary.Add(fromState, startNode); // Insert the node in the open list
		
		while(!openList.IsEmpty)
		{
			AStarMovementNode<State, Transition> node = openList.Dequeue();
			openListDictionary.Remove(node.state);
			
			closedSet.Add(node.state); // Add this node to the closed set
			
			foreach (Transition transition in map.ExpandMovement(node.state, node.steps)) // For every node reachable from this node (transitions)
			{
				State child = map.ApplyTransition(node.state, transition); // Get following state
				
				AStarMovementNode<State, Transition> openListNode = null;
				bool isNodeInFrontier = openListDictionary.TryGetValue(child, out openListNode); // Gets node for state if it was already in the open list

				AStarMovementNode<State, Transition> searchNode = new AStarMovementNode<State, Transition>(map.StepsForState(child, node.steps) - 1, child);

				if (!closedSet.Contains(child) && !isNodeInFrontier) // If following state isn't in the frontier
				{
					openList.Enqueue(searchNode, searchNode.steps);
					openListDictionary.Add(searchNode.state, searchNode);
				}
				else if(isNodeInFrontier)
				{
					if (searchNode.steps > openListNode.steps)
					{
						openList.Replace(openListNode, openListNode.steps, searchNode.steps);
					}
				}
			}
		}

		return closedSet;
	}

	private AStarNode<State, Transition> CreateSearchNode(AStarNode<State, Transition> node, Transition transition, State child, State toState)
	{
		if(node != null)
		{
			float cost = map.PathCost(node.state, transition);
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

		while(searchNode != null)
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