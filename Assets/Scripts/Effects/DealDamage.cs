using BattleSystem;

namespace Effects
{
    public class DealDamage : IEffect
    {
        private readonly int _damage;

        public DealDamage(int damage, TargetingType targetingType)
        {
            _damage = damage;
            TargetingType = targetingType;
        }

        public TargetingType TargetingType { get; }

        public void Perform(Entity author, Entity target)
        {
            target.TakeDamage(_damage);
        }

        public string GetDescription(Entity author)
        {
            switch (TargetingType)
            {
                case TargetingType.AllEnemies:
                    return $"Deal {_damage} damage to all enemies. ";
                case TargetingType.Enemy:
                default:
                    return $"Deal {_damage} damage. ";
            }
        }
    }
}