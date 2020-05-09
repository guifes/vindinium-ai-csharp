using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

class Test
{
    static GameState state;
    static List<ConsoleColor> colors;

    static void Main(string[] args)
    {
        List<IInput> inputs = new List<IInput>();

        inputs.Add(new Player());
        inputs.Add(new Player());
        inputs.Add(new Player());
        inputs.Add(new Player());

        colors = new List<ConsoleColor>();

        colors.Add(ConsoleColor.Red);
        colors.Add(ConsoleColor.Blue);
        colors.Add(ConsoleColor.Green);
        colors.Add(ConsoleColor.Yellow);

        state = new GameState(600);

        string mapInput = ReadEmbeddedTextFile("VindiniumBot.maps.map0.txt");

        StringReader strReader = new StringReader(mapInput);

        state.map = new List<List<bool>>();

        int my = 0;

        string line = strReader.ReadLine();

        int size = line.Length;

        state.size = size;

        string[] map = new string[size];

        while (line != null)
        {
            state.map.Add(new List<bool>(line.Length));

            map[my] = line;
            
            for (int mx = 0; mx < line.Length; mx++)
            {
                char c = line[mx];

                switch (c)
                {
                    case 'M':
                        {
                            Mine mine = new Mine();
                            mine.id = -1;
                            mine.pos = new Vector2i(mx, my);

                            state.mines.Add(mine);
                            state.map[my].Add(false);

                            break;
                        }
                    case '0':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 0;
                            hero.life = 100;
                            hero.pos = new Vector2i(mx, my);
                            hero.spawn = new Vector2i(mx, my);

                            state.heroes[0] = hero;
                            state.map[my].Add(true);

                            break;
                        }
                    case '1':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 1;
                            hero.life = 100;
                            hero.pos = new Vector2i(mx, my);
                            hero.spawn = new Vector2i(mx, my);

                            state.heroes[1] = hero;
                            state.map[my].Add(true);

                            break;
                        }
                    case '2':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 2;
                            hero.life = 100;
                            hero.pos = new Vector2i(mx, my);
                            hero.spawn = new Vector2i(mx, my);

                            state.heroes[2] = hero;
                            state.map[my].Add(true);

                            break;
                        }
                    case '3':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 3;
                            hero.life = 100;
                            hero.pos = new Vector2i(mx, my);
                            hero.spawn = new Vector2i(mx, my);

                            state.heroes[3] = hero;
                            state.map[my].Add(true);

                            break;
                        }
                    case 'T':
                        {
                            Tavern tavern = new Tavern();
                            tavern.pos = new Vector2i(mx, my);

                            state.taverns.Add(tavern);
                            state.map[my].Add(false);

                            break;
                        }
                    case '#':
                        {
                            state.map[my].Add(false);

                            break;
                        }
                    case '.':
                        {
                            state.map[my].Add(true);

                            break;
                        }
                }
            }

            line = strReader.ReadLine();
            my++;
        }

        state.entityCount = state.heroes.Count + state.mines.Count;

        for (int i = 0; i < inputs.Count; i++)
        {
            IInput input = inputs[i];
            input.Start(size, map, i);
        }

        PrintState();

        for (state.round = 0; state.round < state.maxRound; state.round++)
        {
            IInput input = inputs[state.turnId];
            
            Entity[] entities = new Entity[state.entityCount];

            for(int i = 0; i < state.heroes.Count; i++)
            {
                Hero hero = state.heroes[i];

                entities[i].type = "HERO";
                entities[i].id = hero.id;
                entities[i].x = hero.pos.x;
                entities[i].y = hero.pos.y;
                entities[i].life = hero.life;
                entities[i].gold = hero.gold;
            }

            for(int i = 0; i < state.mines.Count; i++)
            {
                Mine mine = state.mines[i];

                int entityIndex = state.heroes.Count + i;

                entities[entityIndex].type = "MINE";
                entities[entityIndex].id = mine.id;
                entities[entityIndex].x = mine.pos.x;
                entities[entityIndex].y = mine.pos.y;
            }

            string action = input.Turn(entities);

            Hero turnHero = state.heroes[state.turnId];

            if (action.Substring(0, 4) == "MOVE")
            {
                string[] parts = action.Split(' ');

                if (parts.Length != 3)
                {
                    throw new Exception("Wrong number of parameters for MOVE");
                }

                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);

                List<Vector2i> path = state.pathfinder.getShortestPath(turnHero.pos, new Vector2i(x, y), true);

                if (path.Count > 0)
                    action = Vector2iToDirection(path[0]);
                else
                    action = "WAIT";
            }

            switch (action)
            {
                case "WAIT": break;
                case "EAST": MoveHero(turnHero, turnHero.pos.x + 1, turnHero.pos.y); break;
                case "WEST": MoveHero(turnHero, turnHero.pos.x - 1, turnHero.pos.y);  break;
                case "SOUTH": MoveHero(turnHero, turnHero.pos.x, turnHero.pos.y + 1); break;
                case "NORTH": MoveHero(turnHero, turnHero.pos.x, turnHero.pos.y - 1);  break;
            }

            // Fight

            foreach(Hero hero in state.heroes)
            {
                if (turnHero.id == hero.id) continue;

                Vector2i distance = turnHero.pos - hero.pos;

                if(distance.size == 1)
                {
                    hero.life -= 20;

                    if(hero.life <= 0)
                    {
                        Die(hero, turnHero);
                    }
                }
            }

            // Win gold

            foreach(Mine mine in state.mines)
            {
                if(mine.id == turnHero.id)
                {
                    turnHero.gold++;
                }
            }

            // Starve

            turnHero.life = Math.Max(turnHero.life - 1, 1);

            state.turnId = (state.turnId + 1) % 4;

            // Print

            PrintState();
        }
    }

    static void PrintState()
    {
        for(int y = 0; y < state.size; y++)
        {
            for(int x = 0; x < state.size; x++)
            {
                Vector2i pos = new Vector2i(x, y);
                bool printed = false;

                foreach(Hero hero in state.heroes)
                {
                    if(hero.pos.Equals(pos))
                    {
                        Console.BackgroundColor = colors[hero.id];
                        Console.Write(hero.id);
                        Console.ResetColor();
                        printed = true;
                        break;
                    }
                }

                if (printed) continue;

                foreach(Mine mine in state.mines)
                {
                    if(mine.pos.Equals(pos))
                    {
                        if (mine.id >= 0)
                            Console.BackgroundColor = colors[mine.id];

                        Console.Write('M');
                        Console.ResetColor();
                        printed = true;
                        break;
                    }
                }

                if (printed) continue;

                foreach (Tavern tavern in state.taverns)
                {
                    if (tavern.pos.Equals(pos))
                    {
                        Console.Write('T');
                        printed = true;
                        break;
                    }
                }

                if (printed) continue;

                if(state.map[y][x])
                {
                    Console.Write('.');
                }
                else
                {
                    Console.Write('#');
                }
            }

            Console.Write('\n');
        }

        Console.Write('\n');
        Console.WriteLine("Round: " + state.round);
        Console.WriteLine("(Hero 1) Life: " + state.heroes[0].life + " Gold: " + state.heroes[0].gold);
        Console.WriteLine("(Hero 2) Life: " + state.heroes[1].life + " Gold: " + state.heroes[1].gold);
        Console.WriteLine("(Hero 3) Life: " + state.heroes[2].life + " Gold: " + state.heroes[2].gold);
        Console.WriteLine("(Hero 4) Life: " + state.heroes[3].life + " Gold: " + state.heroes[3].gold);
        Console.Write('\n');
        Console.ReadLine();
    }

    static void MoveHero(Hero hero, int x, int y)
    {
        Vector2i newPos = new Vector2i(x, y);

        foreach(Hero iHero in state.heroes)
        {
            if (hero.id == iHero.id) continue;

            if(newPos.Equals(iHero.pos))
            {
                return;
            }
        }

        if (!state.map[y][x])
        {
            foreach(Tavern tavern in state.taverns)
            {
                if(newPos.Equals(tavern.pos) && hero.gold >= 2)
                {
                    hero.life = Math.Min(hero.life + 50, 100);
                    hero.gold -= 2;
                }
            }

            foreach (Mine mine in state.mines)
            {
                if (newPos.Equals(mine.pos))
                {
                    if(mine.id != hero.id && hero.life > 20)
                    {
                        hero.life -= 20;
                        mine.id = hero.id;
                    }
                    else
                    {
                        Die(hero);
                    }
                }
            }

            return;
        }

        hero.pos.x = x;
        hero.pos.y = y;
    }

    static void Die(Hero hero, Hero killer = null)
    {
        hero.life = 100;
        hero.pos = hero.spawn;

        foreach(Mine mine in state.mines)
        {
            if(mine.id == hero.id)
            {
                mine.id = killer == null ? -1 : killer.id;
            }
        }

        foreach(Hero h in state.heroes)
        {
            if (hero.id == h.id) continue;

            if(hero.pos.Equals(h.pos))
            {
                Die(h);
            }
        }
    }

    static string Vector2iToDirection(Vector2i v)
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

    static string ReadEmbeddedTextFile(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream(name))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}