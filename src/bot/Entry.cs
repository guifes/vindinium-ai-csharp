using System;

public class Entry
{
    static void Main(string[] args)
    {
        int rounds = 1;

        IInput input = new Player();

        int size = int.Parse(Console.ReadLine());

        string[] map = new string[size];

        for (int i = 0; i < size; i++)
        {
            string line = Console.ReadLine();

            Console.Error.WriteLine(line);

            map[i] = line;
        }

        int myId = int.Parse(Console.ReadLine()); // ID of your hero

        input.Start(size, map, myId);
        
        while (true)
        {
            int entityCount = int.Parse(Console.ReadLine()); // the number of entities

            Entity[] entities = new Entity[entityCount];

            for (int i = 0; i < entityCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');

                entities[i].type = inputs[0]; // HERO or MINE
                entities[i].id = int.Parse(inputs[1]); // the ID of a hero or the owner of a mine
                entities[i].x = int.Parse(inputs[2]); // the x position of the entity
                entities[i].y = int.Parse(inputs[3]); // the y position of the entity
                entities[i].life = int.Parse(inputs[4]); // the life of a hero (-1 for mines)
                entities[i].gold = int.Parse(inputs[5]); // the gold of a hero (-1 for mines)
            }

            string action = input.Turn(rounds++, entities);

            Console.WriteLine(action);
        }
    }
}