public struct Entity
{
    public string type;
    public int id;
    public int x;
    public int y;
    public int life;
    public int gold;
}

public interface IInput
{
    void Start(int size, string[] map, int heroId);
    string Turn(int round, Entity[] entities);
}