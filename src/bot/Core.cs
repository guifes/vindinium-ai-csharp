using System;
using System.Collections.Generic;

class Core : IPathfinder<Vector2i, Vector2i>
{
    AStarPathfinder<Vector2i, Vector2i> pathfinder;
    public Hero myHero = null;
    public List<Hero> heroes = new List<Hero>();
    public List<Mine> mines = new List<Mine>();
    public List<Tavern> taverns = new List<Tavern>();
    public List<List<bool>> map = new List<List<bool>>();
    public Dictionary<Vector2i, int> distanceToHero;
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
            hero.pos = new Vector2i(x, y);
            hero.life = life;
            hero.gold = gold;

            if (hero.id == myId)
            {
                myHero = hero;
            }
        }
        else if (entityType == "MINE")
        {
            Mine mine = mines[mineCount++];

            mine.id = id;
            mine.pos = new Vector2i(x, y);
        }
    }

    public void Process()
    {
        // Processing heroes
        {
            foreach(Hero hero in heroes)
            {
                if (hero.id == myHero.id) continue;

                List<Vector2i> path = pathfinder.getShortestPath(myHero.pos, hero.pos);
                distanceToHero[hero.pos] = path.Count - 1;
            }
        }

        // Processing taverns
        {
            foreach (Tavern tavern in taverns)
            {
                List<Vector2i> path = pathfinder.getShortestPath(myHero.pos, tavern.pos);
                distanceToHero[tavern.pos] = path.Count - 1;

                if (nearestTavern == null || distanceToHero[tavern.pos] < distanceToHero[nearestTavern.pos])
                {
                    nearestTavern = tavern;
                }
            }
        }

        // Processing mines
        {
            foreach (Mine mine in mines)
            {
                List<Vector2i> path = pathfinder.getShortestPath(myHero.pos, mine.pos);
                distanceToHero[mine.pos] = path.Count - 1;

                if (
                    mine.id != myId &&
                    (
                        nearestUnclaimedMine == null ||
                        distanceToHero[mine.pos]< distanceToHero[nearestUnclaimedMine.pos]
                    )
                )
                {
                    nearestUnclaimedMine = mine;
                }
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
                    distanceToHero[nearestTavern.pos] == 0 &&
                    myHero.life <= 75
                ) ||
                (
                    nearestUnclaimedMine != null &&
                    myHero.life - distanceToHero[nearestUnclaimedMine.pos] <= 20
                )
            )
        )
        {
            return "MOVE " + nearestTavern.pos.x + " " + nearestTavern.pos.y;
        }
        else
        {
            if (nearestUnclaimedMine == null)
            {
                if (distanceToHero[nearestTavern.pos] == 0)
                {
                    return "WAIT";
                }
                else
                {
                    return "MOVE " + nearestTavern.pos.x + " " + nearestTavern.pos.y;
                }
            }
            else
            {
                return "MOVE " + nearestUnclaimedMine.pos.x + " " + nearestUnclaimedMine.pos.y;
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