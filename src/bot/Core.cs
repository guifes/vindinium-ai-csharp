using System;
using System.Collections.Generic;

class Core : IPathfinder<Vector2i, Vector2i>
{
    AStarPathfinder<Vector2i, Vector2i> pathfinder;
    public Hero myHero;
    public List<Hero> heroes;
    public List<Mine> mines;
    public List<Tavern> taverns;
    public List<List<bool>> map;
    public Dictionary<Vector2i, int> distanceToHero;
    public HashSet<Vector2i> blockedTiles;
    public Tavern nearestTavern;
    public Mine nearestUnclaimedMine;
    public int size;
    public int myId;
    
    private int mineCount = 0;

    List<Vector2i> transitions = new List<Vector2i>(4);

    public Core()
    {
        pathfinder = new AStarPathfinder<Vector2i, Vector2i>(this);
        heroes = new List<Hero>();
        mines = new List<Mine>();
        taverns = new List<Tavern>();
        map = new List<List<bool>>();
        blockedTiles = new HashSet<Vector2i>();

        transitions.Add(new Vector2i(1, 0));
        transitions.Add(new Vector2i(0, 1));
        transitions.Add(new Vector2i(-1, 0));
        transitions.Add(new Vector2i(0, -1));

        distanceToHero = new Dictionary<Vector2i, int>();
    }

    public void StartTurn()
    {
        mineCount = 0;
        nearestTavern = null;
        nearestUnclaimedMine = null;

        distanceToHero.Clear();
        blockedTiles.Clear();
    }

    public void UpdateEntity(string entityType, int id, int x, int y, int life, int gold)
    {
        if (entityType == "HERO")
        {
            Hero hero = heroes[id];

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

        // Processing possible movements
        {
            foreach(Vector2i t in transitions)
            {
                Vector2i newPosition = myHero.pos + t;
                bool isBlocked = false;

                if (!(MapContains(newPosition) && map[newPosition.y][newPosition.x])) continue;

                foreach (Hero hero in heroes)
                {
                    if(hero.pos.Equals(newPosition))
                    {
                        blockedTiles.Add(newPosition);
                        isBlocked = true;
                    }
                }

                if (isBlocked) continue;

                foreach(Vector2i t2 in transitions)
                {
                    Vector2i nearbyPosition = newPosition + t;

                    if(nearbyPosition.Equals(myHero.pos)) continue;
                    if (!(MapContains(nearbyPosition) && map[nearbyPosition.y][nearbyPosition.x])) continue;

                    foreach (Hero hero in heroes)
                    {
                        int distance = (hero.pos - nearbyPosition).size;

                        if (
                            (
                                hero.pos.Equals(newPosition) &&
                                hero.life - myHero.life > 18
                            ) ||
                            (
                                distance == 1 &&
                                myHero.life - hero.life > 2
                            )
                        )
                        {
                            blockedTiles.Add(newPosition);
                        }
                    }
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

        if(blockedTiles.Count > 0) Console.Error.WriteLine("Blocked Tiles:");

        foreach (Vector2i tile in blockedTiles)
            Console.Error.WriteLine(tile);

        foreach (Vector2i t in transitions)
        {
            Vector2i newPosition = t + myHero.pos;

            foreach (Hero hero in heroes)
            {
                if (hero.id == myHero.id) continue;

                int heroMines = 0;

                foreach (Mine mine in mines)
                {
                    if (mine.id == hero.id) heroMines++;
                }

                int distance = (newPosition - hero.pos).size;

                if (
                    distance == 1 &&
                    heroMines > 0 &&
                    hero.life - myHero.life > 18 &&
                    !blockedTiles.Contains(newPosition)
                )
                {
                    Console.Error.WriteLine("Seeking fight towards hero " + hero.id);
                    return StepTowards(newPosition);
                }
            }
        }

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
            Console.Error.WriteLine("Seeking nearest tavern at " + nearestTavern.pos);
            return StepTowards(nearestTavern.pos);
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
                    Console.Error.WriteLine("Seeking nearest tavern at " + nearestTavern.pos);
                    return StepTowards(nearestTavern.pos);
                }
            }
            else
            {
                Console.Error.WriteLine("Seeking nearest mine at " + nearestUnclaimedMine.pos);
                return StepTowards(nearestUnclaimedMine.pos);
            }
        }
    }

    public string StepTowards(Vector2i destination)
    {
        List<Vector2i> path = pathfinder.getShortestPath(myHero.pos, destination);

        if (path.Count > .0)
            return Vector2iToDirection(path[0]);
        else
            return "WAIT";
    }

    public bool MapContains(Vector2i pos)
    {
        return (pos.x >= 0) &&
               (pos.y >= 0) &&
               (pos.x < size) &&
               (pos.y < size);
    }

    string Vector2iToDirection(Vector2i v)
    {
        if (v.x > 0)
            return "EAST";
        else if (v.x < 0)
            return "WEST";
        else if (v.y > 0)
            return "SOUTH";
        else if (v.y < 0)
            return "NORTH";

        return "WAIT";
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
                MapContains(newState) &&
                (
                    newState.Equals(toState) ||
                    (
                        map[newState.y][newState.x] &&
                        !blockedTiles.Contains(newState)
                    )
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