using System.IO;
using System.Collections.Generic;

public class ReplayBot : IInput
{
    string[] cmd;
    List<string> rounds;

    public ReplayBot(int id, string match)
    {
        rounds = new List<string>();

        StringReader reader = new StringReader(match);

        int round;
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            string[] elements = line.Split(' ');

            switch(elements.Length)
            {
                case 1:
                    reader.ReadLine();

                    round = int.Parse(elements[0]);

                    int tround = ((round - 1) % 4) + 1;
                    int tid = id + 1;

                    if (tround == tid)
                    {
                        if (cmd[0] == "MOVE")
                            rounds.Add(cmd[0] + " " + cmd[1] + " " + cmd[2]);
                        else
                            rounds.Add(cmd[0]);
                    }
                    
                    break;
                case 3:
                    switch(elements[0])
                    {
                        case "EAST":
                        case "NORTH":
                        case "WEST":
                        case "SOUTH":
                        case "WAIT":
                        case "MOVE":
                            cmd = elements;
                            break;
                    }
                    break;
            }
        }
    }
    
    public void Start(int size, string[] map, int heroId)
    {

    }

    public string Turn(int round, Entity[] entities)
    {
        return rounds[round / 4];
    }
}