using BattleSystem;

namespace Effects
{
    public class DealDamage : IEffect
    {
        private readonly int _damage;

        public DealDamage(int damage)
        {
            _damage = damage;
        }

        public void Perform(Entity author, Entity target)
        {
            target.TakeDamage(_damage);
        }

        public string GetDescription(Entity author)
        {
            return $"Deal {_damage} damage.";
        }
    }
}