namespace Effects
{
    public class DealDamage : IEffect
    {
        public DealDamage(int damage)
        {
            _damage = damage;
        }

        private readonly int _damage;

        public void Perform(Entity author, Entity target)
        {
            target.TakeDamage(_damage);
        }
    }
}