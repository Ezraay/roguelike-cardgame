using System.Collections.Generic;

namespace BattleSystem
{
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
            if (_nextEncounter == null)
                return new[]
                {
                    new Enemy(40),
                    new Enemy(50),
                    new Enemy(40)
                };
            return new[]
            {
                new Enemy(30),
                new Enemy(20),
                new Enemy(20)
            };
        }

        public bool HasNextEncounter()
        {
            return _nextEncounter != null;
        }
    }
}