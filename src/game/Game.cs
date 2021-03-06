﻿using System;
using System.Collections.Generic;
using System.IO;

class Game
{
    const int MAX_ROUNDS = 600;

    static GameState state;
    static List<ConsoleColor> colors;

    static void Main(string[] args)
    {
        string match = EmbeddedUtil.ReadTextFile("VindiniumBot.matches.match6.txt");

        List<IInput> inputs = new List<IInput>();

        inputs.Add(new Player());
        inputs.Add(new ReplayBot(1, match));
        inputs.Add(new ReplayBot(2, match));
        inputs.Add(new ReplayBot(3, match));

        colors = new List<ConsoleColor>();

        colors.Add(ConsoleColor.Red);
        colors.Add(ConsoleColor.Blue);
        colors.Add(ConsoleColor.Green);
        colors.Add(ConsoleColor.Yellow);

        state = new GameState(MAX_ROUNDS);

        string mapInput = EmbeddedUtil.ReadTextFile("VindiniumBot.maps.map6.txt");

        StringReader strReader = new StringReader(mapInput);

        state.map = new List<List<bool>>();

        string[] map = null;
        string line = null;
        int size = 0;
        int lineCount = 0;

        while ((line = strReader.ReadLine()) != null)
        {
            if (map == null)
            {
                size = line.Length;
                map = new string[size];
            }

            map[lineCount++] = line;
        }

        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                state.map.Add(new List<bool>(size));

                line = map[y];
                char c = line[x];

                switch (c)
                {
                    case 'M':
                        {
                            Mine mine = new Mine();
                            mine.id = -1;
                            mine.pos = new Vector2i(x, y);

                            state.mines.Add(mine);
                            state.map[x].Add(false);

                            break;
                        }
                    case '0':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 0;
                            hero.life = 100;
                            hero.pos = new Vector2i(x, y);
                            hero.spawn = new Vector2i(x, y);

                            state.heroes[0] = hero;
                            state.map[x].Add(true);

                            break;
                        }
                    case '1':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 1;
                            hero.life = 100;
                            hero.pos = new Vector2i(x, y);
                            hero.spawn = new Vector2i(x, y);

                            state.heroes[1] = hero;
                            state.map[x].Add(true);

                            break;
                        }
                    case '2':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 2;
                            hero.life = 100;
                            hero.pos = new Vector2i(x, y);
                            hero.spawn = new Vector2i(x, y);

                            state.heroes[2] = hero;
                            state.map[x].Add(true);

                            break;
                        }
                    case '3':
                        {
                            Hero hero = new Hero();
                            hero.gold = 0;
                            hero.id = 3;
                            hero.life = 100;
                            hero.pos = new Vector2i(x, y);
                            hero.spawn = new Vector2i(x, y);

                            state.heroes[3] = hero;
                            state.map[x].Add(true);

                            break;
                        }
                    case 'T':
                        {
                            Tavern tavern = new Tavern();
                            tavern.pos = new Vector2i(x, y);

                            state.taverns.Add(tavern);
                            state.map[x].Add(false);

                            break;
                        }
                    case '#':
                        {
                            state.map[x].Add(false);

                            break;
                        }
                    case '.':
                        {
                            state.map[x].Add(true);

                            break;
                        }
                }
            }
        }

        state.size = size;
        state.entityCount = state.heroes.Count + state.mines.Count;

        for (int i = 0; i < inputs.Count; i++)
        {
            IInput input = inputs[i];
            input.Start(size, map, i);
        }

        PrintState();

        for (state.round = 1; state.round <= state.maxRound; state.round++)
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

            string action = input.Turn(state.round, entities);

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

            bool turnHeroDied = false;

            switch (action)
            {
                case "WAIT": break;
                case "EAST": turnHeroDied = MoveHero(turnHero, turnHero.pos.x + 1, turnHero.pos.y); break;
                case "WEST": turnHeroDied = MoveHero(turnHero, turnHero.pos.x - 1, turnHero.pos.y);  break;
                case "SOUTH": turnHeroDied = MoveHero(turnHero, turnHero.pos.x, turnHero.pos.y + 1); break;
                case "NORTH": turnHeroDied = MoveHero(turnHero, turnHero.pos.x, turnHero.pos.y - 1);  break;
            }

            // Fight

            if (!turnHeroDied)
            {
                foreach (Hero hero in state.heroes)
                {
                    if (turnHero.id == hero.id) continue;

                    Vector2i distance = turnHero.pos - hero.pos;

                    if (distance.size == 1)
                    {
                        hero.life -= 20;

                        if (hero.life <= 0)
                        {
                            Die(hero, turnHero);
                        }
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

            Console.WriteLine("On turn " + state.round + " Hero " + turnHero.id + " action: " + action);
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

                foreach (Hero hero in state.heroes)
                {
                    if (hero.spawn.Equals(pos))
                    {
                        Console.ForegroundColor = colors[hero.id];
                        Console.Write("O");
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
                            Console.ForegroundColor = colors[mine.id];
                        else
                            Console.ForegroundColor = ConsoleColor.DarkGray;

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
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('T');
                        Console.ResetColor();
                        printed = true;
                        break;
                    }
                }

                if (printed) continue;

                if(state.map[x][y])
                {
                    Console.Write(' ');
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

        foreach(Hero hero in state.heroes)
        {
            int mines = 0;

            foreach (Mine mine in state.mines)
                if (mine.id == hero.id)
                    mines++;

            Console.BackgroundColor = colors[hero.id];
            Console.Write(" ");
            Console.ResetColor();
            Console.Write(" ");

            Console.Write("(Hero " + hero.id + ") ");
            Console.Write("Life: " + hero.life + " ");
            Console.Write("Gold: " + hero.gold + " ");
            Console.WriteLine("Mines: " + mines);
        }
        
        Console.Write('\n');

        //Console.ReadLine();
    }

    static bool MoveHero(Hero hero, int x, int y)
    {
        Vector2i newPos = new Vector2i(x, y);

        foreach(Hero iHero in state.heroes)
        {
            if (hero.id == iHero.id) continue;

            if(newPos.Equals(iHero.pos))
            {
                return false;
            }
        }

        if (
            (newPos.x < 0) ||
            (newPos.y < 0) ||
            (newPos.x >= state.size) ||
            (newPos.y >= state.size) ||
            !state.map[newPos.x][newPos.y]
        )
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
                if (newPos.Equals(mine.pos) && mine.id != hero.id)
                {
                    if (hero.life > 20)
                    {
                        hero.life -= 20;
                        mine.id = hero.id;
                    }
                    else
                    {
                        Die(hero);

                        return true;
                    }
                }
            }

            return false;
        }

        hero.pos.x = x;
        hero.pos.y = y;

        return false;
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
                Die(h, hero);
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
}