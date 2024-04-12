namespace BattleSystem
{
    public class Level
    {
        private readonly Encounter[] _encounters;
        private readonly string _id;
        
        public Level(string id, Encounter[] encounters)
        {
            _id = id;
            _encounters = encounters;
            CurrentEncounter = _encounters[0];
        }

        public Encounter CurrentEncounter { get; private set; }
        private int _currentEncounterIndex = 0;

        public void Advance()
        {
            CurrentEncounter = _encounters[++_currentEncounterIndex];
        }

        public bool CanAdvance()
        {
            return _currentEncounterIndex < _encounters.Length - 1;
        }
    }
}