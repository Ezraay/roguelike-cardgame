public class Level
{
    public Level(Encounter startingEncounter)
    {
        CurrentEncounter = startingEncounter;
    }

    public Encounter CurrentEncounter { get; private set; }

    public void Advance()
    {
        CurrentEncounter = CurrentEncounter.GetNextEncounter();
    }

    public bool CanAdvance()
    {
        return CurrentEncounter.HasNextEncounter();
    }
}