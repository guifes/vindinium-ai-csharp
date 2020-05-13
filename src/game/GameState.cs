using System;
using System.Collections.Generic;

public class GameState : IPathfinder<Vector2i, Vector2i>
{
    public int round;
    public int maxRound;
    public int turnId;
    public int entityCount;
    public int size;
    public List<Hero> heroes;
    public List<Vector2i> spawns;
    public List<Mine> mines;
    public List<Tavern> taverns;
    public List<List<bool>> map = new List<List<bool>>();
    public AStarPathfinder<Vector2i, Vector2i> pathfinder;

    List<Vector2i> transitions = new List<Vector2i>(4);

    public GameState(int maxRound)
    {
        this.maxRound = maxRound;
        this.turnId = 0;
        this.entityCount = 0;
        this.heroes = new List<Hero>(4);
        this.spawns = new List<Vector2i>(4);
        this.mines = new List<Mine>();
        this.taverns = new List<Tavern>();

        this.transitions.Add(new Vector2i(1, 0));
        this.transitions.Add(new Vector2i(0, 1));
        this.transitions.Add(new Vector2i(-1, 0));
        this.transitions.Add(new Vector2i(0, -1));

        this.heroes.Add(null);
        this.heroes.Add(null);
        this.heroes.Add(null);
        this.heroes.Add(null);

        this.pathfinder = new AStarPathfinder<Vector2i, Vector2i>(this);
    }

    /////////////////
    // IPathfinder //
    /////////////////

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
                    map[newState.y][newState.x] ||
                    newState.Equals(toState)
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