using UnityEngine;

namespace Effects
{
    public class DealDamage : IEffect
    {
        [SerializeField] private readonly int _damage;

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