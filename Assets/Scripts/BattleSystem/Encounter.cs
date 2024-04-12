using System.Collections.Generic;

namespace BattleSystem
{
    public class Encounter
    {
        private Enemy[] _enemies;

        public Encounter(Enemy[] enemies)
        {
            _enemies = enemies;
        }

        public IEnumerable<Enemy> GetEnemies()
        {
            return _enemies;
        }
    }
}