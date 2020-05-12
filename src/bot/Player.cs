using System;
using System.Collections.Generic;

public class Player : IInput
{
    Core core;

    public Player()
    {
        core = new Core();
    }

    public void Start(int size, string[] map, int heroId)
    {
        core.size = size;

        for (int i = 0; i < size; i++)
        {
            string line = map[i];

            Console.Error.WriteLine(line);

            core.map.Add(new List<bool>());

            for (int j = 0; j < line.Length; j++)
            {
                char c = line[j];

                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                        {
                            Hero hero = new Hero();
                            hero.spawn = new Vector2i(j, i);

                            core.heroes.Add(hero);
                            break;
                        }
                    case 'T':
                        {
                            Tavern tavern = new Tavern();
                            tavern.pos = new Vector2i(j, i);

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

        core.myId = heroId;
    }

    public string Turn(int round, Entity[] entities)
    {
        core.StartTurn();
        
        for (int i = 0; i < entities.Length; i++)
        {
            Entity entity = entities[i];

            string type = entity.type;  // HERO or MINE
            int id = entity.id;         // the ID of a hero or the owner of a mine
            int x = entity.x;           // the x position of the entity
            int y = entity.y;           // the y position of the entity
            int life = entity.life;     // the life of a hero (-1 for mines)
            int gold = entity.gold;     // the gold of a hero (-1 for mines)

            core.UpdateEntity(type, id, x, y, life, gold);
        }

        core.Process();

        string action = core.GetAction();

        return action;
    }
}