public class IdleBot : IInput
{
    public void Start(int size, string[] map, int heroId)
    {
    
    }

    public string Turn(int round, Entity[] entities)
    {
        return "WAIT";
    }
}