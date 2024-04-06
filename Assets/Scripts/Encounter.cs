using System.Collections.Generic;

public class Encounter
{
    private readonly Encounter _nextEncounter;

    public Encounter(Encounter nextEncounter = null)
    {
        _nextEncounter = nextEncounter;
    }

    public Encounter GetNextEncounter()
    {
        return _nextEncounter;
    }

    public IEnumerable<Enemy> GetEnemies()
    {
        return new[]
        {
            new Enemy(2),
            new Enemy(1)
        };
    }

    public bool HasNextEncounter()
    {
        return _nextEncounter != null;
    }
}