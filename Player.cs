using System;
//using System.Linq;
//using System.IO;
//using System.Text;
//using System.Collections;
using System.Collections.Generic;

namespace VindiniumBot
{
    class Hero
    {
        public int id;
        public int x;
        public int y;
        public int life;
        public int gold;
    }

    class Mine
    {
        public int id;
        public int x;
        public int y;
    }

    class Tavern
    {
        public int x;
        public int y;
    }

    /**
     * Four legendary heroes were fighting for the land of Vindinium
     * Making their way in the dangerous woods
     * Slashing goblins and stealing gold mines
     * And looking for a tavern where to drink their gold
     **/
    class Player
    {
        static void Main(string[] args)
        {
            Hero myHero = null;
            List<Hero> heroes = new List<Hero>();
            List<Mine> mines = new List<Mine>();
            List<Tavern> taverns = new List<Tavern>();

            for (int i = 0; i < 4; i++)
            {
                heroes.Add(new Hero());
                mines.Add(new Mine());
            }

            int size = int.Parse(Console.ReadLine());

            for (int i = 0; i < size; i++)
            {
                string line = Console.ReadLine();

                for (int j = 0; j < line.Length; j++)
                {
                    char c = line[j];

                    if (c == 'T')
                    {
                        Tavern tavern = new Tavern();
                        tavern.y = i;
                        tavern.x = j;

                        taverns.Add(tavern);
                    }
                }
            }

            int myId = int.Parse(Console.ReadLine()); // ID of your hero

            // game loop
            while (true)
            {
                int entityCount = int.Parse(Console.ReadLine()); // the number of entities
                int mineCount = 0;
                int heroCount = 0;

                for (int i = 0; i < entityCount; i++)
                {
                    string[] inputs = Console.ReadLine().Split(' ');
                    string entityType = inputs[0]; // HERO or MINE

                    if (entityType == "HERO")
                    {
                        Hero hero = heroes[heroCount++];

                        hero.id = int.Parse(inputs[1]); // the ID of a hero or the owner of a mine
                        hero.x = int.Parse(inputs[2]); // the x position of the entity
                        hero.y = int.Parse(inputs[3]); // the y position of the entity
                        hero.life = int.Parse(inputs[4]); // the life of a hero (-1 for mines)
                        hero.gold = int.Parse(inputs[5]); // the gold of a hero (-1 for mines)

                        if (hero.id == myId)
                        {
                            myHero = hero;
                        }
                    }
                    else if (entityType == "MINE")
                    {
                        Mine mine = mines[mineCount++];

                        mine.id = int.Parse(inputs[1]); // the ID of a hero or the owner of a mine
                        mine.x = int.Parse(inputs[2]); // the x position of the entity
                        mine.y = int.Parse(inputs[3]); // the y position of the entity
                    }
                }

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                if (myHero.life <= 50)
                {
                    Console.WriteLine("MOVE " + taverns[0].x + " " + taverns[0].y); // WAIT | NORTH | EAST | SOUTH | WEST
                }
                else
                {
                    Mine nextMine = mines[0];

                    foreach (Mine mine in mines)
                    {
                        if (mine.id != myId)
                        {
                            nextMine = mine;
                            break;
                        }
                    }

                    Console.WriteLine("MOVE " + nextMine.x + " " + nextMine.y); // WAIT | NORTH | EAST | SOUTH | WEST
                }
            }
        }
    }
}