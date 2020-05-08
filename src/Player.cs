using System;
using System.Collections.Generic;

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